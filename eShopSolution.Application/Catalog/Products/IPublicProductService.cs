using eShopSolution.Application.Catalog.DataTranferObjects;
using eShopSolution.Application.Catalog.Products.DataTranferObjects;
using eShopSolution.Application.Catalog.Products.DataTranferObjects.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllByCategoryId(GetProductPagingRequest request);
    }
}
