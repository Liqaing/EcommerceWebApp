using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Utilities.IUtil
{
	
	public interface IStripe
	{
		public string PublishableKey { get; set; }
		public string SecretKey { get; set; }

		public void test();
	}

	
}
