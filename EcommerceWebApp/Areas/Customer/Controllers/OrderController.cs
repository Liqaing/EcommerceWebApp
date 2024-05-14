﻿using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace EcommerceWebApp.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

		public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/api/customer/order")]
        public IActionResult GetAll()
        {
            string appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList = _unitOfWork.OrderHeader.GetAll(
                order => order.AppUserId == appUserId);

            orderHeaderList.OrderByDescending(order => order.OrderHeaderId);

            return Json(new { data = orderHeaderList });
        }

        [HttpGet]
        public IActionResult Detail(int orderId)
        {

            OrderVM order = new()
            {
                orderHeader = _unitOfWork.OrderHeader.Get(
                    order => order.OrderHeaderId == orderId,
                    includeProperties: "AppUser"),
                
                orderDetails = _unitOfWork.OrderDetail.GetAll(
                    order => order.OrderHeaderId == orderId,
                    includeProperties: "Product")
            };

            return View(order);
        }

        [HttpPost]
        public IActionResult Pay(OrderHeader orderHeader)
        {	
			SessionService service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
		}

        [HttpPost]
        [Route("/customer/api/order/updateshipping")]
        public IActionResult UpdateShipping(OrderHeader orderHeader)
        {
			// Update order header, only if he paid
			OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(
				u => u.OrderHeaderId == orderHeader.OrderHeaderId);

			orderHeaderFromDb.Name = orderHeader.Name;
			orderHeaderFromDb.PhoneNumber = orderHeader.PhoneNumber;
			orderHeaderFromDb.HomeNumber = orderHeader.HomeNumber;
			orderHeaderFromDb.StreetName = orderHeader.StreetName;
			orderHeaderFromDb.Village = orderHeader.Village;
			orderHeaderFromDb.Commune = orderHeader.Commune;
			orderHeaderFromDb.City = orderHeader.City;
			orderHeaderFromDb.PostalNumber = orderHeader.PostalNumber;

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            return Json( new { 
                success = true,
                name = orderHeaderFromDb.Name,
                phnomeNumber = orderHeaderFromDb.PhoneNumber,
                homenumber = orderHeaderFromDb.HomeNumber,
                streetNumber = orderHeaderFromDb.StreetName,
                village = orderHeaderFromDb.Village,
				commune = orderHeaderFromDb.Commune,
                city = orderHeaderFromDb.City,
                postalNumber = orderHeaderFromDb.PostalNumber
			});
        }

        [HttpPost]
        public IActionResult Cancel(OrderHeader orderHeader)
        {
            // Cancel order place
            // Check what place order do and revert that


            if (orderHeader.OrderStatus == OrderAndPaymentStatusConstate.StatusCancelled ||
				orderHeader.OrderStatus == OrderAndPaymentStatusConstate.StatusForceCancelled)
            {
                TempData["warning"] = $"Your order #{orderHeader.OrderHeaderId} is already cancelled";
                return RedirectToAction(nameof(Index));
            }

            // Update status
            _unitOfWork.OrderHeader.UpdateStatus(
                orderHeader.OrderHeaderId,
                OrderAndPaymentStatusConstate.StatusCancelled);

            // Add quantity back to product
            IEnumerable<OrderDetail> orderDetails = _unitOfWork.OrderDetail.GetAll(
                u => u.OrderHeaderId == orderHeader.OrderHeaderId,
                includeProperties: "Product"
                );

            foreach (OrderDetail orderDetail in orderDetails )
            {
                orderDetail.Product.Quantity += orderDetail.Quantity;
                _unitOfWork.Product.Update(orderDetail.Product);
            }

            _unitOfWork.Save();

			TempData["success"] = $"Your order #{orderHeader.OrderHeaderId} is successfully cancelled";
            return RedirectToAction(nameof(Index));
        }
    }
}
