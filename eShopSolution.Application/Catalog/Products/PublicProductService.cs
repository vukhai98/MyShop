using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _context;

        public PublicProductService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductViewModel>> GetAllByCategoryId(GetPublicProductPagingRequest request)
        {
            // 1 Select join

            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };

            // 2. Filter

            if (request.CategoryId.Value > 0 && request.CategoryId.HasValue)
            {
                query = query.Where(p => p.pic.CategoryId == request.CategoryId);
            }

            // 3. Pagging 

            var totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                            .Take(request.PageSize)
                            .Select(x => new ProductViewModel()
                            {
                                Id = x.p.Id,
                                Name = x.pt.Name,
                                Price = x.p.Price,
                                OriginalPrice = x.p.OriginalPrice,
                                DateCreated = x.p.DateCreated,
                                SeoAlias = x.pt.SeoAlias,
                                SeoDescription = x.pt.SeoDescription,
                                SeoTitle = x.pt.SeoTitle,
                                LanguageId = x.pt.LanguageId,
                                Stock = x.p.Stock,
                                ViewCount = x.p.ViewCount,
                                Description = x.pt.Description,
                                Details = x.pt.Details
                            }).ToListAsync();

            // 4. Select and projection

            var pagedResult = new PagedResult<ProductViewModel>()
            {
                Items = data,
                TotalRecord = totalRow,

            };

            return pagedResult;

        }
    }
}
