using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EcommerceWebAppProject.Models
{
    public class ShoppingCart
    {
        [Key]
        public int cartId { get; set; }
       
        public int productId { get; set;}        
        [Required]
        [ForeignKey("productId")]
        public Product product { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity need to be greater than 0")]
        public int quantity { get; set; }

        public string appUserId { get; set; }
        [Required]
        [ForeignKey("appUserId")]
        public AppUser appUser { get; set; }

        public double unitPrice { get; set; }
        public double totalPrice { get; set; }
    
        public string shoppingCartStatus { get; set; }

        [NotMapped]
        public int MaxQuantity { get; set; }
    }
}
