﻿using System;
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

		public string? Description { get; set; }
		
		public string? OriginCountry { get; set;}

		[Required(ErrorMessage = "Product price is required")]
		[Range(1, double.MaxValue, ErrorMessage = "Invalid price, product need to be greater than 0")]
		public decimal Price { get; set; }

		public int catId { get; set; }
		[ForeignKey("catId")]
		[Required(ErrorMessage = "Product must be associate with one category")]
        public Category category { get; set; }

		public String ImageUrl { get; set; }
	}
}
