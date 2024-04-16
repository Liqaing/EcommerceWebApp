using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EcommerceWebApp.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    [Authorize]
    public class CartController : Controller
    {
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }

        private readonly IUnitOfWork _unitOfWork;       
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            shoppingCartVM = new()
            {
                shoppingCarts = _unitOfWork.ShoppingCart.GetAll(cart => cart.appUserId == userId,
                    includeProperties: "product.Category"),
                orderHeader = new OrderHeader()
            };

            // Calculate total price of the cart
            //calculateCartPrice(shoppingCartVM);

            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {                
                shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;
            }

            return View(shoppingCartVM);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            shoppingCartVM = new()
            {
                shoppingCarts = _unitOfWork.ShoppingCart.GetAll(cart => cart.appUserId == userId,
                    includeProperties: "product.Category"),
                orderHeader = new OrderHeader()
            };

            // Add user to order header
            shoppingCartVM.orderHeader.AppUser = _unitOfWork.AppUser.Get(user => user.Id == userId);
            shoppingCartVM.orderHeader.Name = shoppingCartVM.orderHeader.AppUser.Name;
            shoppingCartVM.orderHeader.PhoneNumber = shoppingCartVM.orderHeader.AppUser.PhoneNumber;
            shoppingCartVM.orderHeader.HomeNumber = shoppingCartVM.orderHeader.AppUser.HomeNumber;
            shoppingCartVM.orderHeader.StreetName = shoppingCartVM.orderHeader.AppUser.StreetName;
            shoppingCartVM.orderHeader.Village = shoppingCartVM.orderHeader.AppUser.Village;
            shoppingCartVM.orderHeader.Commune = shoppingCartVM.orderHeader.AppUser.Commune;
            shoppingCartVM.orderHeader.City = shoppingCartVM.orderHeader.AppUser.City;
            shoppingCartVM.orderHeader.PostalNumber = shoppingCartVM.orderHeader.AppUser.PostalNumber;

			// Calculate total price of the cart
			//calculateCartPrice(shoppingCartVM);           

			foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
			{
				shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;
			}

			return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            shoppingCartVM.shoppingCarts = _unitOfWork.ShoppingCart.GetAll(cart => cart.appUserId == userId,
                    includeProperties: "product.Category");

            shoppingCartVM.orderHeader.OrderDate = System.DateTime.Now;

			// Add user to order header
			shoppingCartVM.orderHeader.AppUserId = userId;
            shoppingCartVM.orderHeader.AppUser = _unitOfWork.AppUser.Get(user => user.Id == userId);

            // Add status
            shoppingCartVM.orderHeader.OrderStatus = OrderAndPaymentStatusConstate.StatusPending;
			shoppingCartVM.orderHeader.PaymentStatus = OrderAndPaymentStatusConstate.PaymentStatusPending;

			// Calculate total price of the cart			          
			foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
			{
				shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;
			}
            // Save order header
            _unitOfWork.OrderHeader.Add(shoppingCartVM.orderHeader);
            _unitOfWork.Save();

            // Order details

			return View(shoppingCartVM);
		}

		#region api

		[HttpPost]
        public IActionResult Minus(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
                cart => cart.cartId == cartId, includeProperties: "product");

            if (cart.quantity <= 1)
            {
                _unitOfWork.ShoppingCart.Delete(cart);
            }
            else
            {
                cart.quantity -= 1;
                cart.totalPrice = new ShoppingCartUtils().GetTotalPrice(cart);

                _unitOfWork.ShoppingCart.Update(cart);
            }
            _unitOfWork.Save();

            return Json(new { complete = true });
        }

        [HttpPost]
        public IActionResult Add(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
                cart => cart.cartId == cartId, includeProperties: "product");
            
            cart.quantity += 1;
            cart.totalPrice = new ShoppingCartUtils().GetTotalPrice(cart);

            _unitOfWork.ShoppingCart.Update(cart);
            _unitOfWork.Save();

            return Json(new { complete = true });
        }

        [HttpPost]
        public IActionResult Remove(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
                cart => cart.cartId == cartId);

            _unitOfWork.ShoppingCart.Delete(cart);
            _unitOfWork.Save();

            return Json(new { complete = true });
        }

        #endregion

        #region utils

        /*
        private void calculateCartPrice(ShoppingCartVM shoppingCartVM)
        {
            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                cart.totalPrice = getTotalPrice(cart);
                shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;
            }
        }
        

        private double getTotalPrice(ShoppingCart cart) { 
			return cart.quantity * cart.product.Price;
		}
        */
        #endregion
    }
}
