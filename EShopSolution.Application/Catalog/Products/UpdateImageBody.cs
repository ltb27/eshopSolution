namespace EShopSolution.Application.Catalog.Products
{
    public class UpdateImageBody
    {
        public int ImageId { get; set; }
        public string Caption { get; set; }
        public bool IsDefault { get; set; }
    }
}