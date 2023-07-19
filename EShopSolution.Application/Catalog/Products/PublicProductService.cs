using EShopSolution.Application.Catalog.Products.Dto;
using EShopSolution.Application.Catalog.Products.Dto.Manage;
using EShopSolution.Application.Catalog.Products.Dto.Public;
using EShopSolution.Application.Extension.Query;
using EShopSolution.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext db;

        public PublicProductService(EShopDbContext db)
        {
            this.db = db;
        }

        public async Task<PagedResult<ProductViewModel>> GetAllByCategoryId(
            GetPublicProductPagingRequest request
        )
        {
            // query and include loading
            var query = db.Products.AsQueryable();

            query = query
                .Include(q => q.ProductTranslations)
                .Include(q => q.ProductInCategories)
                .ThenInclude(pc => pc.Category);

            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(
                    q => q.ProductInCategories.Any(pc => request.CategoryId.Value == pc.CategoryId)
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
    }
}
