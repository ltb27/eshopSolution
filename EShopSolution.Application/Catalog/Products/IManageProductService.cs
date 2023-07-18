using EShopSolution.Application.Catalog.Products.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductEditRequest request);

        Task<int> Delete(int productId);

        Task<List<ProductViewModel>> GetAll();

        Task<List<PagedViewModel<ProductViewModel>>> GetAlPaging(
            string keyword,
            int pageIndex,
            int pageSize
        );
    }
}
