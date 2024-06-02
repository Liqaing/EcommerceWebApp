using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Stripe.Checkout;
using System.Data;
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
            return Json(new
            {
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

            foreach (OrderDetail orderDetail in orderDetails)
            {
                orderDetail.Product.Quantity += orderDetail.Quantity;
                _unitOfWork.Product.Update(orderDetail.Product);
            }

            _unitOfWork.Save();

            TempData["success"] = $"Your order #{orderHeader.OrderHeaderId} is successfully cancelled";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult DownloadSummary()
        {
            string appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList = _unitOfWork.OrderHeader.GetAll(
                order => order.AppUserId == appUserId).OrderByDescending(order => order.OrderHeaderId);

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

                foreach (var orderHeader in orderHeaderList)
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

        [HttpGet]
        public IActionResult DownlaodDetail()
        {
            string appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            IEnumerable<OrderDetail> orderDetails = _unitOfWork.OrderDetail.GetAll(
                order => order.OrderHeader.AppUserId == appUserId,
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
    }
}
