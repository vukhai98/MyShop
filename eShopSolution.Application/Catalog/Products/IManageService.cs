using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
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

        Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task AddViewCount(int productId);

        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task<int> AddImages(int productId,ProductImageCreateRequest request);

        Task<int> RemoveImages(int imageId);

        Task<int> UpdateImages(int imageId, ProductImageUpdateRequest request);

        Task<List<ProductImageViewModel>> GetListImage(int productId);

        Task<ProductImageViewModel> GetImageById(int imageId);
    }
}
