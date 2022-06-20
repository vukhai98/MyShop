using eShopSolution.Application.Catalog.DataTranferObjects;
using eShopSolution.Application.Catalog.Products.DataTranferObjects;
using eShopSolution.Application.Catalog.Products.DataTranferObjects.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageService
    {
        Task<int> Creat(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);

        Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task AddViewCount(int productId);

        Task<bool> UpdateStock(int productId, int addedQuantity);
    }
}
