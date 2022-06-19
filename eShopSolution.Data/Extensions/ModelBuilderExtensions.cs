using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuiler)
        {
            modelBuiler.Entity<AppConfig>().HasData(
                    new AppConfig() { Key = "HomeTitle", Value = "This is home page of eShopSolution" },
                    new AppConfig() { Key = "HomeKeyword", Value = "This is key word of eShopSolution" },
                    new AppConfig() { Key = "HomeDescription", Value = "This is description of eShopSolution" }
                );

            modelBuiler.Entity<Language>().HasData(
                    new Language() { Id = "vi-Vn", Name = "Tiếng Việt", IsDefault = true },
                    new Language() { Id = "en-US", Name = "English", IsDefault = false }
                    );

            modelBuiler.Entity<Category>().HasData(
                       new Category()
                       {
                           Id = 1,
                           IsShowHome = true,
                           ParentId = null,
                           SortOrder = 1,
                           Status = Status.Active
                       },
                        new Category()
                        {
                            Id = 2,
                            IsShowHome = true,
                            ParentId = null,
                            SortOrder = 2,
                            Status = Status.Active
                        }
                    );

            modelBuiler.Entity<CategoryTranslation>().HasData(
                        new CategoryTranslation() { Id = 1, CategoryId = 1, Name = "Áo nam", LanguageId = "vi-Vn", SeoAlias = "ao-nam", SeoDescription = " Sản phẩm áo thời trang nam việt tiến", SeoTitle = " Sản phẩm áo thời trang nam việt tiến" },
                        new CategoryTranslation() { Id = 2, CategoryId = 1, Name = "Men Shirt", LanguageId = "en-Us", SeoAlias = " t-shirt-men ", SeoDescription = "This is T-Shirt men of Viet Tien", SeoTitle = "This is T-Shirt men of Viet Tien" },
                        new CategoryTranslation() { Id = 3, CategoryId = 2, Name = "Áo nữ", LanguageId = "vi-Vn", SeoAlias = "ao-nu", SeoDescription = " Sản phẩm áo thời trang nữ việt tiến", SeoTitle = " Sản phẩm áo thời trang nữ việt tiến" },
                        new CategoryTranslation() { Id = 4, CategoryId = 2, Name = "Women Shirt", LanguageId = "en-Us", SeoAlias = "t-shirt-women", SeoDescription = "This is T-Shirt women of Viet Tien", SeoTitle = "This is T-Shirt women of Viet Tien" }
                );

            modelBuiler.Entity<Product>().HasData(
                    new Product()
                    {
                        Id = 1,
                        DateCreated = DateTime.Now,
                        OriginalPrice = 100000,
                        Price = 200000,
                        Stock = 0,
                        ViewCount = 0

                    });

            modelBuiler.Entity<ProductTranslation>().HasData(
                    new ProductTranslation()
                    {
                        Id = 1,
                        ProductId = 1,
                        Name = "Áo sơ mi trắng Việt Tiến",
                        LanguageId = "vi-Vn",
                        SeoAlias = "ao-so-mi-nam-trang-viet-tien",
                        SeoDescription = "Áo sơ mi nam trắng Việt Tiến",
                        Details = "Áo sơ mi nam trắng Việt Tiến",
                        SeoTitle = "Áo sơ mi nam trắng Việt Tiến",
                        Description = "Áo sơ mi nam trắng Việt Tiến"
                    },
                    new ProductTranslation()
                    {
                        Id = 2,
                        ProductId = 1,
                        Name = "Viet Tien Men T-Shirt",
                        LanguageId = "en-US",
                        SeoAlias = "viet-tien-men-t-shirt",
                        SeoDescription = "Viet Tien Men T-Shirt",
                        Details = "Viet Tien Men T-Shirt",
                        SeoTitle = "Viet Tien Men T-Shirt",
                        Description = "Viet Tien Men T-Shirt",
                    });


            modelBuiler.Entity<ProductInCategory>().HasData(
                    new ProductInCategory() { ProductId = 1, CategoryId = 1 }
                    );

        }
    }
}
