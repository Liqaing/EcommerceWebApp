using EcommerceWebAppProject.DB.Repository;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
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

		
		public IActionResult DownlaodSummary()
		{
            IEnumerable<OrderHeader> orderHeaders = _unitOfWork.OrderHeader.GetAll(
                includeProperties: "AppUser"
                ).OrderByDescending(order => order.OrderHeaderId);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
			{
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Order-Summary");
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Order ID", typeof(string));
                dataTable.Columns.Add("Order Total", typeof(double));
                dataTable.Columns.Add("Order Status", typeof(string));
                dataTable.Columns.Add("Payment Status", typeof(string));
                dataTable.Columns.Add("Order Date", typeof(string));
                dataTable.Columns.Add("Payment Date", typeof(string));
                dataTable.Columns.Add("Shipping Date", typeof(string));
                dataTable.Columns.Add("Arrival Date", typeof(string));
                dataTable.Columns.Add("Name", typeof(string));
                dataTable.Columns.Add("Phone Number", typeof(string));
                dataTable.Columns.Add("Home Number", typeof(string));
                dataTable.Columns.Add("Street Name", typeof(string));
                dataTable.Columns.Add("Village", typeof(string));
                dataTable.Columns.Add("Commune", typeof(string));
                dataTable.Columns.Add("City", typeof(string));
                dataTable.Columns.Add("Postal Number", typeof(string));
                dataTable.Columns.Add("Delivery Employee", typeof(string));
                dataTable.Columns.Add("Cancel By", typeof(string));

                foreach (var orderHeader in orderHeaders)
                {
                    dataTable.Rows.Add(
                        orderHeader.OrderHeaderId,
                        orderHeader.OrderTotal,
                        orderHeader.OrderStatus,
                        orderHeader.PaymentStatus,
                        orderHeader.OrderDate,
                        orderHeader.PaymentDate,
                        orderHeader.ShippingDate,
                        orderHeader.ArrivalDate,         
                        orderHeader.Name,
                        orderHeader.PhoneNumber,
                        orderHeader.HomeNumber,
                        orderHeader.StreetName,
                        orderHeader.Village,
                        orderHeader.Commune,
                        orderHeader.City,
                        orderHeader.PostalNumber,
                        orderHeader.deliveryEmpName,
                        orderHeader.cancelBy
                    );
                }

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Order-Summary.xlsx");
            }
        }

        public IActionResult DownlaodDetail()
        {
            IEnumerable<OrderDetail> orderDetails = _unitOfWork.OrderDetail.GetAll(
                includeProperties: "Product,OrderHeader,Product.Category"
                ).OrderByDescending(order => order.OrderDetailsId);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Order-Detail");
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Order Detail ID", typeof(string));
                dataTable.Columns.Add("Order Total", typeof(double));
                dataTable.Columns.Add("Order Quantity", typeof(int));
                dataTable.Columns.Add("Payment Unit Price", typeof(double));
                dataTable.Columns.Add("Total Price", typeof(double));
                dataTable.Columns.Add("Product Name", typeof(string));
                dataTable.Columns.Add("Product Origin Country", typeof(string));
                dataTable.Columns.Add("Category", typeof(string));
                dataTable.Columns.Add("Order Date", typeof(string));
                dataTable.Columns.Add("Order Status", typeof(string));
                dataTable.Columns.Add("Order Payment Status", typeof(string));
                dataTable.Columns.Add("Customer Name", typeof(string));
                dataTable.Columns.Add("Customer Phone Number", typeof(string));

                foreach (var orderDetail in orderDetails)
                {
                    dataTable.Rows.Add(
                        orderDetail.OrderDetailsId,
                        orderDetail.OrderHeaderId,
                        orderDetail.Quantity,
                        orderDetail.UnitPrice,
                        orderDetail.Price,
                        orderDetail.Product.ProName,          
                        orderDetail.Product.OriginCountry,
                        orderDetail.Product.Category.CatName,
                        orderDetail.OrderHeader.OrderDate,
                        orderDetail.OrderHeader.OrderStatus,
                        orderDetail.OrderHeader.PaymentStatus,
                        orderDetail.OrderHeader.Name,
                        orderDetail.OrderHeader.PhoneNumber
                    );
                }

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Order-Detail.xlsx");
            }
        }
        #endregion

    }
}
