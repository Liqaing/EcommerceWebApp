using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;


namespace EcommerceWebApp.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    [Authorize]
    public class CartController : Controller
    {
        [BindProperty]
        public ShoppingCartVM? shoppingCartVM { get; set; }

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

		[HttpPost]
		[Route("/Customer/api/cart/order")]
		public IActionResult SummaryPOST()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			shoppingCartVM.shoppingCarts = _unitOfWork.ShoppingCart.GetAll(cart => cart.appUserId == userId,
					includeProperties: "product.Category");

			shoppingCartVM.orderHeader.OrderDate = System.DateTime.Now;

			// Add user to order header
			shoppingCartVM.orderHeader.AppUserId = userId;
			// AppUser appUser = _unitOfWork.AppUser.Get(user => user.Id == userId);

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
			foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.productId,
					OrderHeaderId = shoppingCartVM.orderHeader.OrderHeaderId,
					Price = cart.totalPrice,
					Quantity = cart.quantity
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}


            // Strip service
            string DOMAIN = "https://localhost:7137";
            var options = new SessionCreateOptions
            {
                // Upon success will probably redirect to order view
                SuccessUrl = $"{DOMAIN}/customer/Cart/Summary",
                CancelUrl = $"{DOMAIN}/Customer/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                var SessionItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.totalPrice * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.product.ProName
                        }
                    },
                    Quantity = cart.quantity
                };

                options.LineItems.Add(SessionItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePayment(shoppingCartVM.orderHeader.OrderHeaderId, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            //this.Response.Headers.AccessControlAllowOrigin = DOMAIN;
            //this.Response.Headers.Add("Location", session.Url);

            HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
            HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

            return Redirect(session.Url);            

            //return Json( new { title = $"Order Id: {shoppingCartVM.orderHeader.OrderHeaderId}", message = "You have ordered successfully."} );
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
