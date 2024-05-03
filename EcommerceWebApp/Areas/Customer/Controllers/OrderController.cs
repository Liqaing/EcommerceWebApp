using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            string appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList = _unitOfWork.OrderHeader.GetAll(
                order => order.AppUserId == appUserId
                );

            
            return View(orderHeaderList.OrderByDescending(order => order.OrderHeaderId));
        }
    }
}
