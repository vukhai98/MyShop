using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.Systems.User
{
    public class UserDeleteRequest
    {
        public Guid Id { set; get; }

        [Display(Name = "Tài khoản")]
        public string UserName { set; get; }
    }
}
