using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
