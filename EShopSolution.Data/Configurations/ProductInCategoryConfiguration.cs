using EShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopSolution.Data.Configurations
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.HasKey(x => new { x.CategoryId, x.ProductId });

            builder
                .HasOne(x => x.Category)
                .WithMany(x => x.ProductInCategories)
                .HasForeignKey(x => x.CategoryId);

            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductInCategories)
                .HasForeignKey(x => x.ProductId);
        }
    }
}