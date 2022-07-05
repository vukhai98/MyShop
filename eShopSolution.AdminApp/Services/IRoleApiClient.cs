using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Systems.Roles;

namespace eShopSolution.AdminApp.Services
{
    public interface IRoleApiClient
    {
        Task<ApiResult<List<RoleViewModel>>> GetAll();
    }
}
