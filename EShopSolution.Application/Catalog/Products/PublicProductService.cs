using System;
using System.Linq;
using System.Threading.Tasks;
using EShopSolution.Application.Extension.Query;
using EShopSolution.Data.Context;
using EshopSolution.PageModel.Catalog.Product.Public;
using EshopSolution.PageModel.Common;
using Microsoft.EntityFrameworkCore;

namespace EShopSolution.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _db;

        public PublicProductService(EShopDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<ProductViewModel>> GetAllByCategoryId(
            GetPublicProductPagingRequest request
        )
        {
            // query and include loading
            var query = _db.Products.AsQueryable();

            query = query
                .Include(q => q.ProductTranslations)
                .Include(q => q.ProductInCategories)
                .ThenInclude(pc => pc.Category);

            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
                query = query.Where(
                    q => q.ProductInCategories.Any(pc => request.CategoryId.Value == pc.CategoryId)
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
    }
}