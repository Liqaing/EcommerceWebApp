using EcommerceWebAppProject.DB.Repository;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = RoleConstant.Role_Admin)]
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

        #region api
        [HttpGet]
        [Route("Admin/api/order/all")]
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(
                includeProperties: "AppUser"
                ).OrderByDescending(order=> order.OrderHeaderId);

            return Json( new { data = orderHeaders });
        }

        #endregion

    }
}
