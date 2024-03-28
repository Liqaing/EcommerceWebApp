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
	public class Product
	{
		[Key]
		public int ProductId { get; set; }
		
		[Required(ErrorMessage = "Product name is required")]
		public string? ProName { get; set; }

        [Required(ErrorMessage = "Product quantity is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Invalid quantity, product quantity need to be greater than 0")]
        public int? Qauntity { get; set; }

        public string? Description { get; set; }
		
		public string? OriginCountry { get; set;}

		[Required(ErrorMessage = "Product price is required")]
		[Range(0, double.MaxValue, ErrorMessage = "Invalid price, product price need to be greater than 0")]
		public decimal Price { get; set; }

		public int CatId { get; set; }
		[ForeignKey("CatId")]		
        public Category Category { get; set; }
		
		public string? ImageUrl { get; set; }
	}
}
