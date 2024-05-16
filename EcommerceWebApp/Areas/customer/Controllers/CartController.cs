using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Net;
using System.Reflection.Metadata.Ecma335;
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
                shoppingCarts = _unitOfWork.ShoppingCart.GetAll(
                    cart => cart.appUserId == userId &&
                    cart.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive,
                    includeProperties: "product.Category"),
                orderHeader = new OrderHeader()
            };

            // Calculate total price of the cart
            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                if (cart.unitPrice != cart.product.Price)
                {
                    cart.unitPrice = cart.product.Price;
                    cart.totalPrice = ShoppingCartUtils.GetTotalPrice(cart);
                }
                shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if product that user have in cart is in stock
            IEnumerable<ShoppingCart> carts = _unitOfWork.ShoppingCart.GetAll(
                    cart => cart.appUserId == userId &&
                    cart.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive,
                    includeProperties: "product"
                );

            if (carts.Count() == 0)
            {                
                return Json(new { success = false, msg = "Sorry, you don't have any item in your shopping cart" });
            }

            foreach (ShoppingCart cart in carts)
            {
                if (cart.quantity > cart.product.Quantity)
                {
                    return Json(new { success = false, msg = $"Sorry, we don't have enough {cart.quantity} quantity to offer for product: {cart.product.ProName}" });
                }
            }

            return Json(new { success = true });
        }

        [HttpGet]
        //[HttpPost]
        public IActionResult Summary()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            shoppingCartVM = new()
            {
                shoppingCarts = _unitOfWork.ShoppingCart.GetAll(
                    cart => cart.appUserId == userId &&
                    cart.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive,
                    includeProperties: "product.Category"),
                orderHeader = new OrderHeader()
            };
            

            if (shoppingCartVM.shoppingCarts.Count() == 0)
            {

                return RedirectToAction(nameof(Index));
                //return Json(new { success = false, msg = "Sorry, you don't have any item in your shopping cart" });
            }

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
			// Checking if we have enough product
			foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
			{
                if (cart.quantity > cart.product.Quantity)
                {
                    return RedirectToAction(nameof(Index));
                    //return Json(new { success = false, msg=$"Sorry, we don't have enough {cart.quantity} quantity to offer for product: {cart.product.ProName}"});
                }

				shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;
			}

            return View(shoppingCartVM);
            //return RedirectToAction(nameof(Checkout));
            //return Json(new { success = true });
        }       

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
                cart.unitPrice = cart.product.Price;
                cart.quantity -= 1;
                cart.totalPrice = ShoppingCartUtils.GetTotalPrice(cart);

                _unitOfWork.ShoppingCart.Update(cart);
            }
            _unitOfWork.Save();

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            HttpContext.Session.SetInt32(OrderAndPaymentStatusConstate.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.appUserId == userId &&
                    u.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive).Count());

            return Json(new { complete = true });
        }

        [HttpPost]
        public IActionResult Add(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
                cart => cart.cartId == cartId, includeProperties: "product");

            cart.unitPrice = cart.product.Price;
            cart.quantity += 1;
            cart.totalPrice = ShoppingCartUtils.GetTotalPrice(cart);

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

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            HttpContext.Session.SetInt32(OrderAndPaymentStatusConstate.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.appUserId == userId &&
                    u.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive).Count());
            
            return Json(new { complete = true });
        }

		[HttpPost]
        //[Route("/Customer/api/cart/order")]
        [ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
            if (!ModelState.IsValid)
            {
                TempData["warning"] = "Sorry, the information you provided is not valid";
                return RedirectToAction(nameof(Summary));
            }

			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			shoppingCartVM.shoppingCarts = _unitOfWork.ShoppingCart.GetAll(
                cart => cart.appUserId == userId &&
                cart.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive,
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
                /*
                cart.shoppingCartStatus = ShoppingCartStatusConstant.StatusOrder;
                _unitOfWork.ShoppingCart.Update(cart);
                */

                // Order Heander
				shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;


                // Update product quantity
                EcommerceWebAppProject.Models.Product product = _unitOfWork.Product.Get(
                    u => u.ProductId == cart.productId);
                product.Quantity -= cart.quantity;
                _unitOfWork.Product.Update(product);
            }

			// Save order header
			_unitOfWork.OrderHeader.Add(shoppingCartVM.orderHeader);
			_unitOfWork.Save();

			// Order Detail
			foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.productId,
                    OrderHeaderId = shoppingCartVM.orderHeader.OrderHeaderId,
                    Price = cart.totalPrice,
                    UnitPrice = cart.unitPrice,
                    Quantity = cart.quantity
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
            }
            _unitOfWork.Save();
             
            // Strip service
            string DOMAIN = "https://localhost:7137";
            var options = new SessionCreateOptions
            {
                // Upon success will probably redirect to order view
                SuccessUrl = $"{DOMAIN}/customer/Cart/OrderConfirm?id={shoppingCartVM.orderHeader.OrderHeaderId}",
                CancelUrl = $"{DOMAIN}/Customer/Cart/OrderConfirm?id={shoppingCartVM.orderHeader.OrderHeaderId}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                var SessionItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(cart.unitPrice * 100),
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

            Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
            
			//return Json( new { title = $"Order Id: {shoppingCartVM.orderHeader.OrderHeaderId}", message = "You have ordered successfully."} );
		}

        
        // Order Success
        public IActionResult OrderConfirm(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(
                order => order.OrderHeaderId == id, includeProperties: "AppUser");

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

			_unitOfWork.OrderHeader.UpdateStripePayment(
					orderHeader.OrderHeaderId,
					session.Id,
					session.PaymentIntentId);

			// Set all cart to order
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			IEnumerable<ShoppingCart> carts = _unitOfWork.ShoppingCart.GetAll(
				cart => cart.appUserId == userId &&
				cart.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive);

			foreach (ShoppingCart cart in carts)
			{
				cart.shoppingCartStatus = ShoppingCartStatusConstant.StatusOrder;
                _unitOfWork.ShoppingCart.Update(cart);
			}

			if (session.PaymentStatus == "paid")
            {
                // handle order and payment sucess               
                _unitOfWork.OrderHeader.UpdateStatus(
                    orderHeader.OrderHeaderId,
                    OrderAndPaymentStatusConstate.StatusApproved,
                    OrderAndPaymentStatusConstate.PaymentStatusApproved);			                

                TempData["success"] = $"Your order #{orderHeader.OrderHeaderId} is successfully placed";                
            }
            else
            {
                // Handle order when payment is pending
				_unitOfWork.OrderHeader.UpdateStatus(
					orderHeader.OrderHeaderId,
					OrderAndPaymentStatusConstate.StatusPending,
					OrderAndPaymentStatusConstate.PaymentStatusPending);

				TempData["warning"] = $"Please proceed with the payment for order #{orderHeader.OrderHeaderId}";
			}
            
            _unitOfWork.Save();
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index), "Order", new { area = "Customer"});
        }

        #region api

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
