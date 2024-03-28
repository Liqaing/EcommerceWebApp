using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EcommerceWebApp.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class HomeController : Controller
    {
        public IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Category> categories = _unitOfWork.Category.GetAll(includeProperties: "Products");
            return View(categories);
        }

        [HttpGet]
		public IActionResult Details(int proId)
		{
            ShoppingCart cart = new()
            {
                product = _unitOfWork.Product.Get(pro => pro.ProductId == proId,
                includeProperties: "Category"),

                productId = proId,
                qauntity = 1
            };

			return View(cart);
		}


        [HttpPost]
        public IActionResult Details(ShoppingCart cart)
        {
            

            return View(cart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}