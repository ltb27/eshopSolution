using EshopSolution.PageModel.Catalog.Product.Manage;

namespace EshopSolution.PageModel.Catalog.Product.Public
{
    public class GetPublicProductPagingRequest : PagingRequestBase
    {
        public int? CategoryId { get; set; }
        public string LanguageId { get; set; }
    }
}