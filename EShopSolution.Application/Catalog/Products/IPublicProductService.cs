using System.Threading.Tasks;
using EShopSolution.Application.Catalog.Products.Dto;
using EShopSolution.Application.Catalog.Products.Dto.Public;

namespace EShopSolution.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllByCategoryId(
            GetPublicProductPagingRequest request
        );
    }
}
