using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceWebApp.Areas.customer.Controllers
{
	[Area(nameof(Customer))]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			ShoppingCartVM shoppingCartVM = new()
			{
				shoppingCarts = _unitOfWork.ShoppingCart.GetAll(cart => cart.appUserId == userId,
				includeProperties: "product.Category")
			};

            // Calculate total price of the cart
			foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
			{
				cart.totalPrice = getTotalPrice(cart);
                shoppingCartVM.OrderTotal += cart.totalPrice;
			}

            return View(shoppingCartVM);
		}

		public IActionResult Minus(int cartId)
		{
			ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
				cart => cart.cartId == cartId);

			if (cart.qauntity <= 1)
			{
				_unitOfWork.ShoppingCart.Delete(cart);
			}
			else
			{
                cart.qauntity -= 1;
                _unitOfWork.ShoppingCart.Update(cart);
            }
            _unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

        public IActionResult Add(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
                cart => cart.cartId == cartId);

            cart.qauntity += 1;
            _unitOfWork.ShoppingCart.Update(cart);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
                cart => cart.cartId == cartId);

            _unitOfWork.ShoppingCart.Delete(cart);            
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        private double getTotalPrice(ShoppingCart cart) { 
			return cart.qauntity * cart.product.Price;
		}
	}
}
