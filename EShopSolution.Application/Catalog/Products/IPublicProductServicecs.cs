using EShopSolution.Application.Catalog.Products.Dtos;

namespace EShopSolution.Application.Catalog.Products
{
    public interface IPublicProductServicecs
    {
        PagedViewModel<ProductViewModel> GetAllByCategoryId(int categoryId);
    }
}
