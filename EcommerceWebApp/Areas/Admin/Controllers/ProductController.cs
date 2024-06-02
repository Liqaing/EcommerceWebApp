using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = $"{RoleConstant.Role_Admin},{RoleConstant.Role_Sale_Employee}")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this._unitOfWork = unitOfWork;
            this._webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(
                includeProperties: "Category").ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Upsert(int? proId)
        {
            ProductVM productVM = new()
            {
                // Get list of category dropdown
                CategoryList = _unitOfWork.Category.GetAll()
                    .Select(cat => new SelectListItem
                    {
                        Text = cat.CatName,
                        Value = cat.CatId.ToString()
                    })
            };

            if (proId == null || proId == 0)
            {
                // Create new product
                productVM.Product = new Product();
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(pro => pro.ProductId == proId);
            }
            return View(productVM);


        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? productImage)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (productImage != null)
                {
                    // Get random 128 bit id to identify the file
                    string imageName = $"{Guid.NewGuid()}{Path.GetExtension(productImage.FileName)}";
                    string imageFolder = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // delete old image and update new one                        
                        DeleteImage(wwwRootPath, productVM.Product);
                    }

                    // Save image
                    string imagePath = Path.Combine(imageFolder, imageName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        productImage.CopyTo(fileStream);
                    };

                    productVM.Product.ImageUrl = $@"\images\product\{imageName}";
                }

                if (productVM.Product.ProductId == 0)
                {
                    this._unitOfWork.Product.Add(productVM.Product);
                    TempData["Success"] = $"Product: {productVM.Product.ProName} created successfully";
                }
                else
                {
                    this._unitOfWork.Product.Update(productVM.Product);
                    TempData["Success"] = $"Product: {productVM.Product.ProName} updated successfully";
                }

                this._unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(cat => new SelectListItem
                    {
                        Text = cat.CatName,
                        Value = cat.CatId.ToString()
                    });
                return View(productVM);
            }

        }

        #region api

        [HttpGet]
        [Route("/Admin/api/product/all")]
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(
                includeProperties: "Category").ToList();          

            return Json(new { data = products });
        }

        [HttpDelete]
        [Route("/Admin/api/product/delete")]
        public IActionResult Delete(int? proId)
        {
            Product product = _unitOfWork.Product.Get(pro => pro.ProductId == proId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product Not Found" });

            }

            DeleteImage(_webHostEnvironment.WebRootPath, product);

            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();

            string successMsg = $"Product: {product.ProName} deleted successfully";
            return Json(new { success = true, message = successMsg });
        }

        
        public IActionResult Download()
        {
             List<Product> products = _unitOfWork.Product.GetAll(
               includeProperties: "Category").ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Products");
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Product Id", typeof(int));
                dataTable.Columns.Add("Product Name", typeof(string));
                dataTable.Columns.Add("Quantity", typeof(int));
                dataTable.Columns.Add("Description", typeof(string));
                dataTable.Columns.Add("Origin Country", typeof(string));
                dataTable.Columns.Add("Price", typeof(double));
                dataTable.Columns.Add("Category Id", typeof(int));
                dataTable.Columns.Add("Category Name", typeof(string));

                foreach (var product in products)
                {
                    dataTable.Rows.Add(
                        product.ProductId,
                        product.ProName,
                        product.Quantity,
                        product.Description,
                        product.OriginCountry,
                        product.Price,
                        product.CatId,
                        product.Category?.CatName
                    );
                }

                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
            }
        }


        #endregion


        #region util

        // Delete image in www root folder
        public void DeleteImage(string wwwRootPath, Product product)
        {  
            if (string.IsNullOrEmpty(product.ImageUrl))
            {
                return;
            }

            string imagePath =
                Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        #endregion
    }
}
