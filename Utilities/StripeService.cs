using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Utilities
{
	public class StripeService
	{
		public string PublishableKey { get; set; }
		public string SecretKey { get; set;}

	}
}
