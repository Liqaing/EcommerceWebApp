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
		public string name { get; set; }

		public string? homeNumber { get; set; }
		public string? streetName { get; set; }
		public string? village { get; set; }
		public string? commune { get; set; }
		public string? city { get; set; }
		public string? postalNumber { get; set; }
	}
}
