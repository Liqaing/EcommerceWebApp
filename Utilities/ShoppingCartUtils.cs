using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EcommerceWebAppProject.Models;

namespace EcommerceWebAppProject.Utilities
{
    public static class ShoppingCartUtils
    {
        public static double GetTotalPrice(ShoppingCart cart)
        {            
            return cart.quantity * cart.unitPrice;            
        }

        public static double GetTotalPrice(int quantity, double price )
        {
            return quantity * price;
        }
    }
}
