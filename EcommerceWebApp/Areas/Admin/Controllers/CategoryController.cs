using EcommerceWebAppProject.Models;
using Microsoft.AspNetCore.Mvc;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.DB.Repository;
using Microsoft.AspNetCore.Authorization;
using EcommerceWebAppProject.Utilities;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
	[Area(nameof(Admin))]
    [Authorize(Roles = RoleConstant.Role_Admin)]
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
        

        #region api

        [HttpGet]
        [Route("/admin/api/category/all")]
        public IActionResult GetAll()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return Json( new {data = categories} );
        }

        [HttpDelete]
        [Route("/admin/api/category/delete")]
        public IActionResult Delete(int? catId) { 
            if (catId == null || catId == 0)
            {
                return Json( new { success = false, message = "Category Not Found" });
            }

            Category cat = _unitOfWork.Category.Get(cat => cat.CatId== catId);
            _unitOfWork.Category.Delete(cat);
            _unitOfWork.Save();
            return Json(new { success = true, message = $"Category: {cat.CatName} deleted successfully"});
        }

        #endregion
    }
}
