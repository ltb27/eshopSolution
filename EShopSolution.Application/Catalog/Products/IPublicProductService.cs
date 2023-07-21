using System.Threading.Tasks;
using EshopSolution.PageModel.Catalog.Product.Public;
using EshopSolution.PageModel.Common;

namespace EShopSolution.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllByCategoryId(
            GetPublicProductPagingRequest request
        );
    }
}