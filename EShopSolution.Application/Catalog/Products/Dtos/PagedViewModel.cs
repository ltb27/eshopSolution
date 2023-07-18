using System.Collections.Generic;

namespace EShopSolution.Application.Catalog.Products.Dtos
{
    public class PagedViewModel<T>
    {
        public int Total { get; set; }
        public List<T> Items { get; set; }
    }
}
