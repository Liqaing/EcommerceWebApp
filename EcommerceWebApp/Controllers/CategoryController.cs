using EcommerceWebApp.Data;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    public class CategoryController : Controller
    {

        private readonly AppDbContext _dbContext;
        public CategoryController(AppDbContext db)
        {
            // Get db context from dependency injection to work with db
            _dbContext = db;
        }

        public IActionResult Index()
        {
            List<Category> categories = _dbContext.Category.ToList();
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
                _dbContext.Category.Add(newCat);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        [HttpGet]
        public IActionResult Edit(int? catId)
        {
            if (catId == null || catId == 0) {
                return StatusCode(404, "Category not found");
            }
            Category? cat = _dbContext.Category.Find(catId);
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
                _dbContext.Category.Update(cat);
                _dbContext.SaveChanges();
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
            Category? cat = _dbContext.Category.Find(catId);
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

            _dbContext.Category.Remove(cat);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
