using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                includeProperties: "category").ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Upsert(int? proId)
        {
            ProductVM productVM = new()
            {                
                // Get list of category dropdown
                categoryList = _unitOfWork.Category.GetAll()
                    .Select(cat => new SelectListItem
                    {
                        Text = cat.CatName,
                        Value = cat.CatId.ToString()
                    })
            };

            if (proId == null || proId == 0)
            {
                // Create new product
                productVM.product = new Product();
            }
            else
            {
                productVM.product = _unitOfWork.Product.Get(pro => pro.ProductId == proId);
            }
            return View(productVM);


        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? productImage)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (productImage != null )
                {
                    // Get random 128 bit id to identify the file
                    string imageName = $"{Guid.NewGuid()}{Path.GetExtension(productImage.FileName)}";
                    string imageFolder = Path.Combine(wwwRootPath, @"images\product");
                    
                    if (!string.IsNullOrEmpty(productVM.product.ImageUrl))
                    {
                        // delete old image and update new one                        
                        deleteImage(wwwRootPath, productVM.product);
                    }

                    // Save image
                    string imagePath = Path.Combine(imageFolder, imageName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        productImage.CopyTo(fileStream);
                    };

                    productVM.product.ImageUrl = $@"\images\product\{imageName}";
                }

                if (productVM.product.ProductId == 0)
                {
                    this._unitOfWork.Product.Add(productVM.product);
                    TempData["Success"] = $"Product: {productVM.product.ProName} created successfully";
                }
                else
                {
                    this._unitOfWork.Product.Update(productVM.product);
                    TempData["Success"] = $"Product: {productVM.product.ProName} updated successfully";
                }

                this._unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.categoryList = _unitOfWork.Category.GetAll()
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
                includeProperties: "category").ToList();

            return Json(new { data = products });
        }

        [HttpDelete]
        [Route("/Admin/api/product/delete")]
        public IActionResult Delete(int? proId)
        {
            Product product = _unitOfWork.Product.Get(pro => pro.ProductId == proId);
            if (product == null)
            {
                return Json( new { success = false, message = "Product Not Found" } );

            }
                
            deleteImage(_webHostEnvironment.WebRootPath, product);

            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();

            string successMsg = $"Product: {product.ProName} deleted successfully";
            return Json( new {success = true, message = successMsg} );
        }

        #endregion


        #region util

        // Delete image in www root folder
        public void deleteImage(string wwwRootPath, Product product)
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
