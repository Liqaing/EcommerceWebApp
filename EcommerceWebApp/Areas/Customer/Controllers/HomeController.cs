using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using EcommerceWebAppProject.Utilities;

namespace EcommerceWebApp.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
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
                quantity = 1
            };
			
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

        #region api

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("/customer/api/cart/add")]
        public IActionResult Details(ShoppingCart cart)
        {
            // Check if product have enough quantity for order
            Product product = _unitOfWork.Product.Get(pro => pro.ProductId == cart.productId);

            if (cart.quantity > product.Quantity)
            {
                return Json(new { success = false, message = $"Sorry, we could provide quatity: {cart.quantity} at the moment" });
            }

            // Get user id
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            cart.appUserId = userId;

            ShoppingCart cartInDb = _unitOfWork.ShoppingCart.Get(
                c => c.productId == cart.productId &&
                c.appUserId == userId &&
                c.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive,
                includeProperties: "product");

            if (cartInDb == null)
            {
                // Shopping cart for that user and product is not exist in the db                
                cart.unitPrice = product.Price;
                cart.totalPrice = new ShoppingCartUtils().GetTotalPrice(cart.quantity, cart.unitPrice);
                cart.shoppingCartStatus = ShoppingCartStatusConstant.StatusActive;
                _unitOfWork.ShoppingCart.Add(cart);
            }
            else
            {
                // Update qauntity in cart
                cart.unitPrice = product.Price;
                cartInDb.quantity += cart.quantity;
                cartInDb.totalPrice = new ShoppingCartUtils().GetTotalPrice(cartInDb);
                _unitOfWork.ShoppingCart.Update(cartInDb);
            }

            _unitOfWork.Save();

            //TempData["Success"] = $"You have added {cart.quantity} {productName} to the shopping cart";

            return Json(new { success = false, message = $"You have added {cart.quantity} {product.ProName} to the shopping cart" });
            //return RedirectToAction(nameof(Index));
        }

        #endregion

        #region util

        #endregion
    }
}