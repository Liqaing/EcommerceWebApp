using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Models
{
	public class AppUser: IdentityUser
	{
		[Required]
		public string Name { get; set; }

		public string? HomeNumber { get; set; }
		public string? StreetName { get; set; }
		public string? Village { get; set; }
		public string? Commune { get; set; }
		public string? City { get; set; }
		public string? PostalNumber { get; set; }
	}
}
