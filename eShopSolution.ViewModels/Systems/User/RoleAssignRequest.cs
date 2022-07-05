using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Systems.User
{
    public class RoleAssignRequest
    {
        public RoleAssignRequest()
        {
            Roles  = new List<SelectItem>();
        }
        public Guid Id { set; get; }

        public List<SelectItem> Roles { set; get; } 
    }
}
