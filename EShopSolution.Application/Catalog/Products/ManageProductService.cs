using EShopSolution.Application.Catalog.Products.Dtos;
using EShopSolution.Data.Context;
using EShopSolution.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext db;

        public ManageProductService(EShopDbContext db)
        {
            this.db = db;
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            Product product = new Product() { Price = request.Price, };
            db.Products.Add(product);

            return await db.SaveChangesAsync();
        }

        public async Task<int> Delete(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductViewModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<List<PagedViewModel<ProductViewModel>>> GetAlPaging(
            string keyword,
            int pageIndex,
            int pageSize
        )
        {
            throw new NotImplementedException();
        }

        public async Task<int> Update(ProductEditRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
