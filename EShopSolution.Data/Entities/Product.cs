using System;
using System.Collections.Generic;

namespace EShopSolution.Data.Entities
{
    public class Product
    {
        public int Id { set; get; }
        public decimal Price { set; get; }
        public decimal OriginalPrice { set; get; }
        public int Stock { set; get; }
        public int ViewCount { set; get; }
        public DateTime DateCreated { set; get; }
        public List<ProductInCategory> ProductInCategories { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
        public virtual List<Cart> Carts { get; set; }
        public virtual List<ProductTranslation> ProductTranslations { get; set; }
    }
}