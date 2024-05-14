using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Http;

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
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId != null)
            {
                HttpContext.Session.SetInt32(OrderAndPaymentStatusConstate.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.appUserId == userId &&
                    u.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive).Count());
            }

            IEnumerable<Category> categories = _unitOfWork.Category.GetAll(includeProperties: "Products");
            return View(categories);
        }

        [HttpGet]	
		public IActionResult Details(int proId)
		{
            Product product = _unitOfWork.Product.Get(u => u.ProductId == proId);

            ShoppingCart cart = new()
            {
                product = _unitOfWork.Product.Get(pro => pro.ProductId == proId,
                includeProperties: "Category"),

                productId = proId,
                quantity = 1,
                MaxQuantity = product.Quantity,
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
        [Route("/customer/api/cart/add")]
        public IActionResult AddCart(int proId, int quantity)
        {
            // Check if product have enough quantity for order
            Product product = _unitOfWork.Product.Get(pro => pro.ProductId == proId);

            if (quantity > product.Quantity)
            {
                /*
                TempData["warning"] = $"Sorry, we could provide quatity: {cart.quantity} at the moment";
                return RedirectToAction(nameof(Details), new { proId = product.ProductId });
                */
                return Json(new { success = false, message = $"Sorry, we could provide quatity: {quantity} at the moment" });
            }

            // Get user id
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCart cart = new ShoppingCart
            {               
                productId = proId,
                quantity = quantity,
                appUserId = userId,
                unitPrice = product.Price,
                totalPrice = ShoppingCartUtils.GetTotalPrice(quantity, product.Price),
                shoppingCartStatus = ShoppingCartStatusConstant.StatusActive
            };

            ShoppingCart cartInDb = _unitOfWork.ShoppingCart.Get(
                c => c.productId == cart.productId &&
                c.appUserId == userId &&
                c.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive,
                includeProperties: "product");

            if (cartInDb == null)
            {
                // Shopping cart for that user and product is not exist in the db                                
                _unitOfWork.ShoppingCart.Add(cart);

                // Add product count to session
                HttpContext.Session.SetInt32(OrderAndPaymentStatusConstate.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.appUserId == userId &&
                    u.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive).Count());
            }
            else
            {
                // Update qauntity in cart                
                cartInDb.quantity += cart.quantity;
                cartInDb.totalPrice = ShoppingCartUtils.GetTotalPrice(cartInDb);
                _unitOfWork.ShoppingCart.Update(cartInDb);
            }

            _unitOfWork.Save();

            //TempData["Success"] = $"You have added {cart.quantity} {product.ProName} to the shopping cart";

            return Json(new { success = true, message = $"You have added {cart.quantity} {product.ProName} to the shopping cart" });
            //return RedirectToAction(nameof(Index));
        }

        #endregion

        #region util

        #endregion
    }
}