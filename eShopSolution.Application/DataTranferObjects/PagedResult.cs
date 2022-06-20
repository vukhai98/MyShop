using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.DataTranferObjects
{
    public class PagedResult<T>
    {
        public List<T> Items { set; get; }

        public int TotalRecord { set; get; }    
    }
}
