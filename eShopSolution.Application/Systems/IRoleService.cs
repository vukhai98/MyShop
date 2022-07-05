using eShopSolution.ViewModels.Systems.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Systems
{
    public interface IRoleService
    {
        Task<ICollection<RoleViewModel>> GetAll();
    }
}
