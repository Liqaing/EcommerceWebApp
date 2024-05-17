using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Models.ViewModel
{
    public class ShoppingCartVM
    {
        [ValidateNever]
        public IEnumerable<ShoppingCart> shoppingCarts { get; set; }
        
        public OrderHeader orderHeader { get; set; }
    }
}
