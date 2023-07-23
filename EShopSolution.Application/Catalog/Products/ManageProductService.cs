using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EShopSolution.Application.Common;
using EShopSolution.Application.Extension.Query;
using EShopSolution.Data.Context;
using EShopSolution.Data.Entities;
using EShopSolution.Entities.Exceptions;
using EshopSolution.PageModel.Catalog.Product;
using EshopSolution.PageModel.Catalog.Product.Manage;
using EshopSolution.PageModel.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext db;
        private readonly IStorageService storageService;

        public ManageProductService(EShopDbContext db, IStorageService fileStorageService)
        {
            this.db = db;
            storageService = fileStorageService;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == productId);
            product.ViewCount += 1;

            await db.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>
                {
                    new ProductTranslation
                    {
                        LanguageId = request.LanguageId,
                        Name = request.Name,
                        Description = request.Description,
                        SeoDescription = request.SeoDescription,
                        SeoAlias = request.SeoAlias,
                        SeoTitle = request.SeoTitle
                    }
                }
            };

            // save image
            if (request.ThumbnailImage != null)
                product.Images = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Caption = "Thumbnail Image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await SaveFile(request.ThumbnailImage),
                        SortOrder = 1
                    }
                };

            db.Products.Add(product);

            return await db.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            var product = await db.Products.Include(x => x.Images).ThenInclude(x => x.ImagePath)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
                throw new EShopException($"Product with {productId} can not found!");
            
            // delete images in the directory
            var imagePaths = product.Images.Select(x => x.ImagePath).ToList();
            imagePaths.ForEach(DeleteImage);
            
            // delete the productImages record in db
            var productImages = await db.ProductImages.Where(x => x.ProductId == productId).ToListAsync();
            
            db.ProductImages.RemoveRange(productImages);
            db.Products.Remove(product);

            return await db.SaveChangesAsync();
        }

        private async void DeleteImage(string filePath)
        {
            await storageService.DeleteFileAsync(filePath.Split("/")[1]);
        }

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(
            GetProductPagingRequest request
        )
        {
            // query and include loading
            var query = db.Products.AsQueryable();

            query = query
                .Include(q => q.ProductTranslations)
                .Include(q => q.ProductInCategories)
                .ThenInclude(pc => pc.Category);

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var searchPattern = "%" + request.Keyword.Trim() + "%";
                query = query.Where(
                    q =>
                        q.ProductTranslations.Any(
                            t =>
                                t.Name.ToLower().Contains(request.Keyword.Trim().ToLower())
                                || EF.Functions.Like(t.Name, searchPattern)
                        )
                );
            }

            if (request.CategoryIds?.Any() == true)
                query = query.Where(
                    q =>
                        q.ProductInCategories.Any(pc => request.CategoryIds.Contains(pc.CategoryId))
                );

            // count
            var total = await query.CountAsync();

            // pagination
            query = query.Pagination(request.PageSize, request.PageIndex);

            // mapping
            var products = await query
                .Select(
                    q =>
                        new
                        {
                            q.Id,
                            q.Price,
                            q.OriginalPrice,
                            q.Stock,
                            q.ViewCount,
                            CreateDate = q.DateCreated,
                            ProductTranslation = q.ProductTranslations.FirstOrDefault(
                                x =>
                                    string.Equals(
                                        x.LanguageId,
                                        request.LanguageId,
                                        StringComparison.OrdinalIgnoreCase
                                    )
                            )
                        }
                )
                .Select(
                    q =>
                        new ProductViewModel
                        {
                            Id = q.Id,
                            Price = q.Price,
                            OriginalPrice = q.OriginalPrice,
                            Stock = q.Stock,
                            ViewCount = q.ViewCount,
                            CreateDate = q.CreateDate,
                            Name = q.ProductTranslation.Name,
                            Description = q.ProductTranslation.Description,
                            Details = q.ProductTranslation.Details,
                            SeoAlias = q.ProductTranslation.SeoAlias,
                            SeoTitle = q.ProductTranslation.SeoTitle,
                            SeoDescription = q.ProductTranslation.SeoDescription
                        }
                )
                .ToListAsync();

            var pagedResult = new PagedResult<ProductViewModel>
            {
                Total = total,
                Items = products
            };

            return pagedResult;
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

            var productTranslation =
                await db.ProductTranslations.FirstOrDefaultAsync(
                    q => q.ProductId == request.Id && q.LanguageId == request.LanguageId
                );

            if (product == null || productTranslation == null)
                throw new EShopException("Product can not found!");

            productTranslation.Name = request.Name;
            productTranslation.SeoAlias = request.SeoAlias;
            productTranslation.SeoTitle = request.SeoTitle;
            productTranslation.SeoDescription = request.SeoDescription;
            productTranslation.Description = request.Description;
            productTranslation.Details = request.Details;

            // save image
            if (request.ThumbnailImage != null)
            {
                var exist = await db.ProductImages.FirstOrDefaultAsync(
                    x => x.IsDefault && x.ProductId == request.Id);

                if (exist == null) return await db.SaveChangesAsync();

                // save the new image
                exist.FileSize = request.ThumbnailImage.Length;

                var spitedFilePath = exist.ImagePath.Split("/");

                var existFileName = spitedFilePath.Length >= 2 ? spitedFilePath[1] : default;

                // delete the old image
                if (existFileName != default) await storageService.DeleteFileAsync(existFileName);

                exist.ImagePath = await SaveFile(request.ThumbnailImage);

                // update the record to new image path
                db.ProductImages.Update(exist);
            }

            return await db.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await db.Products.FindAsync(productId);
            if (product == null)
                throw new EShopException("Product can not found!");

            product.Price = newPrice;

            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await db.Products.FindAsync(productId);

            if (product == null)
                throw new EShopException("Product can not found!");

            product.Price += addedQuantity;

            return await db.SaveChangesAsync() > 0;
        }

        public async Task AddImage(int productId, List<IFormFile> files)
        {
            var product = await db.Products.FindAsync(productId);
            if (product == null)
            {
                throw new EShopException($"Product with Id: {productId} is not found");
            }

            var order = 1;
            foreach (var file in files)
            {
                product.Images.Add(new ProductImage()
                {
                    Caption = "Product Image",
                    DateCreated = DateTime.Now,
                    FileSize = file.Length,
                    ImagePath = await SaveFile(file),
                    SortOrder = order
                });
                order++;
            }

            await db.SaveChangesAsync();
        }

        public async Task RemoveImage(int imageId)
        {
            var productImage = await db.ProductImages.FindAsync(imageId);
            if(productImage == null)
                throw new EShopException();
            
            var fileName = productImage.ImagePath.Split("/")[1];

            await storageService.DeleteFileAsync(fileName);
            db.ProductImages.Remove(productImage);
           await db.SaveChangesAsync();
        }

        public async Task UpdateImage(UpdateImageBody body)
        {
            var productImage = await db.ProductImages.FindAsync(body.ImageId);
            if (productImage == null)
                throw new EShopException($"Image with Id {body.ImageId} is not found");
            db.Entry(productImage).CurrentValues.SetValues(body);

            await db.SaveChangesAsync();
        }

        public async Task<List<ProductImageViewModel>> GetListImage(int productId)
        {
            var product = await db.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null)
                throw new EShopException($"Product with Id: {productId} is not found");

            return product.Images.Select(x => new ProductImageViewModel()
            {
                Id = x.Id,
                FileSize = x.FileSize,
                IsDefault = x.IsDefault,
                FilePath = x.ImagePath
            }).ToList();

        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

            await storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return storageService.GetFileUrl(fileName);
        }
    }
}