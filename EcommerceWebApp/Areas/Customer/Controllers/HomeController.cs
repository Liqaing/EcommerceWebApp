using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

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
		[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
		[Authorize]
		[ValidateAntiForgeryToken]
		public IActionResult Details(ShoppingCart cart, string productName)
		{
			// Get user id
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			cart.appUserId = userId;

            ShoppingCart cartInDb = _unitOfWork.ShoppingCart.Get(
                cart => cart.productId == cart.productId &&
                cart.appUserId == userId);

            if (cartInDb == null)
            {
				// Shopping cart for that user and product is not exist in the db
				_unitOfWork.ShoppingCart.Add(cart);
			}
            else
            {
                // Update qauntity in cart
				cartInDb.qauntity += cart.qauntity;
				_unitOfWork.ShoppingCart.Update(cartInDb);
			}

			_unitOfWork.Save();

			TempData["Success"] = $"You have added {cart.qauntity} {productName} to the shopping cart";

            return RedirectToAction(nameof(Index));
			//return RedirectToAction(nameof(Details), new { proId=cart.productId, succeedMessage=succeedMessage });
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

	#region api

	#endregion
}