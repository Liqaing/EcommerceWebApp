﻿using EcommerceWebAppProject.DB.Repository.IRepository;
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
                    includeProperties: "product.Category"),
                orderHeader = new OrderHeader()
            };

            // Calculate total price of the cart
            calculateCartPrice(shoppingCartVM);

            return View(shoppingCartVM);
        }


        public IActionResult Summary()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartVM shoppingCartVM = new()
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
            calculateCartPrice(shoppingCartVM);

            return View(shoppingCartVM);
        }

        #region api

        [HttpPost]
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

            return Json(new { complete = true });
        }

        [HttpPost]
        public IActionResult Add(int cartId)
        {
            ShoppingCart cart = _unitOfWork.ShoppingCart.Get(
                cart => cart.cartId == cartId);
            cart.qauntity += 1;

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

        private void calculateCartPrice(ShoppingCartVM shoppingCartVM)
        {
            foreach (ShoppingCart cart in shoppingCartVM.shoppingCarts)
            {
                cart.totalPrice = getTotalPrice(cart);
                shoppingCartVM.orderHeader.OrderTotal += cart.totalPrice;
            }
        }

        private double getTotalPrice(ShoppingCart cart) { 
			return cart.qauntity * cart.product.Price;
		}
        #endregion
    }
}
