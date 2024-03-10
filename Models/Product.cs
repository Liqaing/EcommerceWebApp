using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
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
		public decimal Price { get; set; }
	}
}
