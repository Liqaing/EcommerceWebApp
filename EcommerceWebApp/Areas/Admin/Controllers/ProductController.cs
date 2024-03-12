using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

		public ProductController(IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
		}

		public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll().ToList();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product newPro)
        {
            if (ModelState.IsValid)
            {
                this._unitOfWork.Product.Add(newPro);
                this._unitOfWork.Save();
                TempData["Success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            return View();
            
        }


        [HttpGet]
        public IActionResult Edit(int? proId)
        {
            if (proId == null || proId == 0)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Product not found");
            }

            Product? product = _unitOfWork.Product.Get(pro=> pro.ProductId == proId);
            if (product == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Product not found");
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product pro)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Product.Update(pro);
                _unitOfWork.Save();
                TempData["success"] = "Product edited successfully";
                return RedirectToAction("Index");
            }

            return View();
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
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }

    }
}
