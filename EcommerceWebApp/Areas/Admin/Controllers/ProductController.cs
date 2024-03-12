using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using Microsoft.AspNetCore.Mvc;

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

    }
}
