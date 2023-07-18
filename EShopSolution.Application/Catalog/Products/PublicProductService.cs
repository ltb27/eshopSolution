using EShopSolution.Application.Catalog.Products.Dtos;
using System;

namespace EShopSolution.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductServicecs
    {
        public PagedViewModel<ProductViewModel> GetAllByCategoryId(int categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
