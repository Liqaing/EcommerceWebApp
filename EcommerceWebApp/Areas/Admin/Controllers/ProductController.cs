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
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
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
                    string imageName = new Guid().ToString() + Path.GetExtension(productImage.FileName);
                    string imageFolder = Path.Combine(wwwRootPath, @"images/product");
    
                    // Save image
                    using (var fileStream = new FileStream(Path.Combine(imageFolder, imageName), FileMode.Create))
                    {
                        productImage.CopyTo(fileStream);
                    };

                    productVM.product.ImageUrl = $"images/product/{imageName}";
                }
                this._unitOfWork.Product.Add(productVM.product);
                this._unitOfWork.Save();
                TempData["Success"] = $"Product: {productVM.product} created successfully";
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

        [HttpGet]
        public IActionResult Delete(int? proId)
        {
            if (proId == null || proId == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Product not found");
            }

            Product? product = _unitOfWork.Product.Get(pro => pro.ProductId == proId);
            if (product == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Product not found");
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Delete(Product pro)
        {
            if (pro == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Product not found");
            
            }
            _unitOfWork.Product.Delete(pro);
            _unitOfWork.Save();
            TempData["success"] = $"Product: {pro.ProName} deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
