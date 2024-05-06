using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
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
        
        [BindProperty]
        public OrderVM order { get; set; }

		public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            string appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList = _unitOfWork.OrderHeader.GetAll(
                order => order.AppUserId == appUserId);
            
            return View(orderHeaderList.OrderByDescending(order => order.OrderHeaderId));
        }

        [HttpGet]
        public IActionResult Detail(int orderId)
        {

            order = new()
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
        public IActionResult Pay()
        {	
			SessionService service = new SessionService();
            Session session = service.Get(order.orderHeader.SessionId);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
		}

        [HttpPost]
        [Route("/customer/api/order/updateshipping")]
        public IActionResult UpdateShipping()
        {
			// Update order header, only if he paid
			OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(
				u => u.OrderHeaderId == order.orderHeader.OrderHeaderId);

			orderHeaderFromDb.Name = order.orderHeader.AppUser.Name;
			orderHeaderFromDb.PhoneNumber = order.orderHeader.AppUser.PhoneNumber;
			orderHeaderFromDb.HomeNumber = order.orderHeader.AppUser.HomeNumber;
			orderHeaderFromDb.StreetName = order.orderHeader.AppUser.StreetName;
			orderHeaderFromDb.Village = order.orderHeader.AppUser.Village;
			orderHeaderFromDb.Commune = order.orderHeader.AppUser.Commune;
			orderHeaderFromDb.City = order.orderHeader.AppUser.City;
			orderHeaderFromDb.PostalNumber = order.orderHeader.AppUser.PostalNumber;

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);

            return Json(new { "success" = true });
        }
    }
}
