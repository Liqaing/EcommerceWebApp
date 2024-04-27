using EcommerceWebAppProject.Utilities.IUtil;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Utilities
{
	public class Stripe: IStripe
	{
		public string PublishableKey { get; set; }
		public string SecretKey { get; set;}

		public Stripe(string publishableKey, string secretKey) { 
			PublishableKey = publishableKey;
			SecretKey = secretKey;
		}

		public void test()
		{
			var options = new RequestOptions
			{
				StripeAccount = "acct_1032D82eZvKYlo2C"
			};
			var service = new ChargeService();
			Charge charge = service.Get(
			  "ch_3LmjRC2eZvKYlo2C1vvMTc1Q",
			  options
			);
		}

	}
}
