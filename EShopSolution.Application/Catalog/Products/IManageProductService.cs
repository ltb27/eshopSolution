using EShopSolution.Application.Catalog.Products.Dto.Manage;
using EShopSolution.Application.Catalog.Products.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);

        Task AddViewCount(int productId);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task<bool> UpdateStock(int productId, int changingQuantity);
    }
}
