using System.Collections.Generic;
using EshopSolution.PageModel.Catalog.Product.Manage;

namespace EshopSolution.PageModel.Catalog.Product
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
        public List<int> CategoryIds { get; set; }

        public string LanguageId { get; set; }
    }
}