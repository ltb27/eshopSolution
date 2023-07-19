using EShopSolution.Application.Catalog.Products.Dto;
using EShopSolution.Application.Catalog.Products.Dto.Manage;
using EShopSolution.Data.Context;
using EShopSolution.Data.Entities;
using EShopSolution.Entities.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EShopSolution.Application.Extension.Query;

namespace EShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext db;

        public ManageProductService(EShopDbContext db)
        {
            this.db = db;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == productId);
            product.ViewCount += 1;

            await db.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            Product product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
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

            db.Products.Add(product);

            return await db.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            Product product = await db.Products.FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
                throw new EShopException($"Product with {product.Id} can not found!");

            db.Products.Remove(product);

            return await db.SaveChangesAsync();
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
            {
                query = query.Where(
                    q =>
                        q.ProductInCategories.Any(pc => request.CategoryIds.Contains(pc.CategoryId))
                );
            }

            // count
            int total = await query.CountAsync();

            // pagination
            query = query.Pagination(request.pageSize, request.pageIndex);

            // mapping
            List<ProductViewModel> products = await query
                .Select(
                    q =>
                        new
                        {
                            Id = q.Id,
                            Price = q.Price,
                            OriginalPrice = q.OriginalPrice,
                            Stock = q.Stock,
                            ViewCount = q.ViewCount,
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

            PagedResult<ProductViewModel> pagedResult = new PagedResult<ProductViewModel>()
            {
                Total = total,
                Items = products
            };

            return pagedResult;
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            Product product = await db.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

            ProductTranslation productTranslation =
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
    }
}
