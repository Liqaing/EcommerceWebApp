using EcommerceWebAppProject.Models;
using Microsoft.AspNetCore.Mvc;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.DB.Repository;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
    {        
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            // Get db context from dependency injection to work with db
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Create new category
        [HttpPost]
        public IActionResult Create(Category newCat)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(newCat);
                _unitOfWork.Save();
                TempData["success"] = $"Category: {newCat.CatName} created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet]
        public IActionResult Edit(int? catId)
        {
            if (catId == null || catId == 0)
            {
                return StatusCode(404, "Category not found");
            }
            Category? cat = _unitOfWork.Category.Get(cat => cat.CatId == catId);
            if (cat == null)
            {
                return StatusCode(404, "Category not found");
            }

            return View(cat);
        }

        // Update category
        [HttpPost]
        public IActionResult Edit(Category cat)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(cat);
                _unitOfWork.Save();
                TempData["success"] = $"Category: {cat.CatName} edited successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int? catId)
        {
            if (catId == null || catId == 0)
            {
                return StatusCode(404, "Category not found");
            }
            Category? cat = _unitOfWork.Category.Get(cat => cat.CatId == catId);
            if (cat == null)
            {
                return StatusCode(404, "Category not found");
            }

            return View(cat);
        }

        // Delete category
        [HttpPost]
        public IActionResult Delete(Category cat)
        {

            if (cat == null)
            {
                return StatusCode(404, "Category not found");
            }

            _unitOfWork.Category.Delete(cat);
            _unitOfWork.Save();
            TempData["success"] = $"Category: {cat.CatName} deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
