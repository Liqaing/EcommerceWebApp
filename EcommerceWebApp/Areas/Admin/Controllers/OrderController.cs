using EcommerceWebAppProject.DB.Repository;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]    
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
		[Authorize(Roles = $"{RoleConstant.Role_Admin},{RoleConstant.Role_Sale_Employee},{RoleConstant.Role_Delivery_Employee}")]
		public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		[Authorize(Roles = RoleConstant.Role_Delivery_Employee)]
		public IActionResult DeliveryOrder()
		{
			return View();
		}

		[HttpGet]
		[Authorize(Roles = $"{RoleConstant.Role_Admin},{RoleConstant.Role_Sale_Employee},{RoleConstant.Role_Delivery_Employee}")]
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
		[Authorize(Roles = $"{RoleConstant.Role_Admin},{RoleConstant.Role_Sale_Employee}")]
		public IActionResult Cancel(OrderHeader orderHeader)
		{
			// Cancel placed order
			if (orderHeader.OrderStatus == OrderAndPaymentStatusConstate.StatusCancelled ||
				orderHeader.OrderStatus == OrderAndPaymentStatusConstate.StatusForceCancelled)
			{
				TempData["warning"] = $"The order #{orderHeader.OrderHeaderId} is already cancelled";
				return RedirectToAction(nameof(Index));
			}

			// Update status
			_unitOfWork.OrderHeader.UpdateStatus(
				orderHeader.OrderHeaderId,
				OrderAndPaymentStatusConstate.StatusForceCancelled);

			// Add quantity back to product
			IEnumerable<OrderDetail> orderDetails = _unitOfWork.OrderDetail.GetAll(
				u => u.OrderHeaderId == orderHeader.OrderHeaderId,
				includeProperties: "Product"
				);

			foreach (OrderDetail orderDetail in orderDetails)
			{
				orderDetail.Product.Quantity += orderDetail.Quantity;
				_unitOfWork.Product.Update(orderDetail.Product);
			}

			_unitOfWork.Save();

			TempData["success"] = $"The order #{orderHeader.OrderHeaderId} is successfully cancelled";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[Authorize(Roles = RoleConstant.Role_Delivery_Employee)]
		public IActionResult Pick(OrderHeader orderHeader)
		{
			// Check if order can be pick up for delivery
			if (orderHeader.OrderStatus != OrderAndPaymentStatusConstate.StatusApproved || 
				orderHeader.PaymentStatus != OrderAndPaymentStatusConstate.PaymentStatusApproved)
			{
				TempData["warning"] = $"You cannot pick order #{orderHeader.OrderHeaderId} for delivery";
				return RedirectToAction(nameof(Index));
			}

			OrderHeader orderFromDb = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == orderHeader.OrderHeaderId);

			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			AppUser deliveryEmp = _unitOfWork.AppUser.Get(u => u.Id == userId);

            orderFromDb.DeliveryEmpId = deliveryEmp.Id;
            orderFromDb.deliveryEmpName = deliveryEmp.Name;

			orderFromDb.ShippingDate = DateTime.Now;
			orderFromDb.OrderStatus = OrderAndPaymentStatusConstate.StatusInDelivering;

			_unitOfWork.OrderHeader.Update(orderFromDb);
			_unitOfWork.Save();

			TempData["success"] = $"The order #{orderFromDb.OrderHeaderId} is successfully picked for delivery";
			return RedirectToAction(nameof(Index));
		}


        [HttpPost]
        [Authorize(Roles = RoleConstant.Role_Delivery_Employee)]
        public IActionResult Drop(OrderHeader orderHeader)
        {
            // Check if order can be pick up for delivery
            if (orderHeader.OrderStatus != OrderAndPaymentStatusConstate.StatusInDelivering)
            {
                TempData["warning"] = $"You cannot drop order #{orderHeader.OrderHeaderId} as delivered";
                return RedirectToAction(nameof(Index));
            }

            OrderHeader orderFromDb = _unitOfWork.OrderHeader.Get(u => u.OrderHeaderId == orderHeader.OrderHeaderId);                  

            orderFromDb.ArrivalDate = DateTime.Now;
            orderFromDb.OrderStatus = OrderAndPaymentStatusConstate.StatusDelivered;

            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Save();

            TempData["success"] = $"The order #{orderFromDb.OrderHeaderId} is successfully dilivered to customer";
            return RedirectToAction(nameof(Index));
        }

        #region api
        [HttpGet]
		[Authorize(Roles = $"{RoleConstant.Role_Admin},{RoleConstant.Role_Sale_Employee},{RoleConstant.Role_Delivery_Employee}")]
		[Route("Admin/api/order/all")]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(
                includeProperties: "AppUser"
                ).OrderByDescending(order=> order.OrderHeaderId);

            return Json( new { data = orderHeaders });
        }

        [HttpGet]
		[Authorize(Roles = RoleConstant.Role_Delivery_Employee)]
		[Route("Admin/api/order/delivery/all")]
        public IActionResult OrderForDelievryEmp()
        {
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(
                u=> u.DeliveryEmpId == userId,
				includeProperties: "AppUser"
                ).OrderByDescending(order=> order.OrderHeaderId);

            return Json( new { data = orderHeaders });
        }

        #endregion

    }
}
