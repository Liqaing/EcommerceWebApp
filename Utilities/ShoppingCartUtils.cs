using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EcommerceWebAppProject.Models;

namespace EcommerceWebAppProject.Utilities
{
    public class ShoppingCartUtils
    {
        public double GetTotalPrice(ShoppingCart cart)
        {
            return cart.quantity * cart.product.Price;
        }

        public double GetTotalPrice(int quantity, double price )
        {
            return quantity * price;
        }
    }
}
