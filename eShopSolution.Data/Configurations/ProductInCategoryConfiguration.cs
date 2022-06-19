using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Data.Configurations
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.ToTable("ProductInCategories");

            //Many to many

            builder.HasKey(x => new { x.ProductId, x.CategoryId });

            builder.HasOne(x => x.Product).WithMany(pc => pc.ProductInCategories)
                   .HasForeignKey(pc => pc.ProductId);

            builder.HasOne(x => x.Category).WithMany(pc => pc.ProductInCategories)
                  .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
