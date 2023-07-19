using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShopSolution.Application.Catalog.Products.Dto.Manage;

namespace EShopSolution.Application.Catalog.Products.Dto
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
        public List<int> CategoryIds { get; set; }

        public string LanguageId { get; set; }
    }
}
