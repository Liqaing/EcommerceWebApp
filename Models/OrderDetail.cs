using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailsId { get; set; }
        
        [Required]
        public int OrderHeaderId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(OrderHeaderId))]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public int Quantity { get; set; }
		public double UnitPrice { get; set; }
		public double Price { get; set; }
    }
}
