using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Areas.customer.Controllers
{
	[Area(nameof(Customer))]
	public class CartController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
