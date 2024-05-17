using EcommerceWebAppProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Models.ViewModel
{
    public class OrderVM
    {
        public OrderHeader orderHeader { get; set; }
        public IEnumerable<OrderDetail> orderDetails { get; set; }
    }
}
