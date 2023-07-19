using EShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EShopSolution.Data.Configurations
{
    public class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTranslation> builder)
        {
            // table config
            builder.ToTable("ProductTranslations");

            // primary key config
            builder.HasKey(X => X.Id);

            // property config
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.SeoAlias).IsRequired().HasMaxLength(200);
            builder.Property(X => X.Details).HasMaxLength(500);
            builder.Property(x => x.LanguageId).IsUnicode(false).IsRequired().HasMaxLength(5);

            // constrains config
            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductTranslations)
                .HasForeignKey(x => x.ProductId);

            builder
                .HasOne(x => x.Language)
                .WithMany(x => x.ProductTranslations)
                .HasForeignKey(x => x.LanguageId);
        }
    }
}
