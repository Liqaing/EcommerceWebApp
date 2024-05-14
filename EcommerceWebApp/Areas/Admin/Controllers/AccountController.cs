using EcommerceWebAppProject.DB.Data;
using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Models.ViewModel;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceWebApp.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = RoleConstant.Role_Admin)]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _appDbContext;
        public AccountController(IUnitOfWork unitOfWork,
            RoleManager<UserRole> roleManager,
            UserManager<AppUser> userManager,
            AppDbContext db)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
            _appDbContext = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        /*
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<AppUser> users = _userManager.Users;
            List<UserVM> allUser = new List<UserVM>();

            foreach (var user in users)
            {             
                var roles = await _userManager.GetRolesAsync(user);

                UserVM userVM = new()
                {
                    appUser = user,
                    Role = roles.FirstOrDefault()
                };

                allUser.Add(userVM);
            }

            return Json(new { users = allUser });
        }
        */

        #region api

        [HttpGet]
        [Route("/admin/api/account/all")]
        public IActionResult GetAll()
        {
            List<AppUser> users = _unitOfWork.AppUser.GetAll().ToList();
            List<UserRole> roles = _unitOfWork.Role.GetAll().ToList();

            // Get data from AspUserRole tb
            var userRole = _appDbContext.UserRoles.ToList();

            foreach (AppUser user in users)
            {
                // Look through db to find role for the user
                string roleId = userRole.FirstOrDefault(
                    u => u.UserId == user.Id).RoleId;

                user.roleId = roleId;
                user.role = roles.FirstOrDefault(u => u.Id == roleId).Name;

            }

            return Json(new { data = users });
        }

        /*
        [HttpDelete]
        [Route("/admin/api/category/delete")]
        public IActionResult Delete(int? catId)
        {
            if (catId == null || catId == 0)
            {
                return Json(new { success = false, message = "Category Not Found" });
            }

            Category cat = _unitOfWork.Category.Get(cat => cat.CatId == catId);
            _unitOfWork.Category.Delete(cat);
            _unitOfWork.Save();
            return Json(new { success = true, message = $"Category: {cat.CatName} deleted successfully" });
        }
        */

        [HttpGet]
        public IActionResult Detail(string id)
        {
            AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == id);
            appUser.roleId = _appDbContext.UserRoles.Where(u => u.UserId == id).FirstOrDefault().RoleId;
            appUser.role = _unitOfWork.Role.Get(u => u.Id == appUser.roleId).Name;

            UserVM userVM = new()
            {
                appUser = appUser,
                RoleList = _unitOfWork.Role.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id
                    }

                )
            };

            return View(userVM);
        }

        [HttpPost]
        public IActionResult LockUnlock(AppUser appUser)
        {
            //AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == id);
            if (appUser == null)
            {
                //return Json(new { sucess = false, msg = "Error while locking/unlocking user" });
                TempData["warning"] = "Error while locking/unlocking user";
                return RedirectToAction(nameof(Index));
            }

            string state = "";
            if (appUser.LockoutEnd != null && appUser.LockoutEnd > DateTime.Now)
            {
                // user is currently lock, so unlock them
                appUser.LockoutEnd = DateTime.Now;
                state = "unlocked";
            }
            else
            {
                appUser.LockoutEnd = DateTime.Now.AddYears(100);
                state = "locked";
            }
            _unitOfWork.AppUser.Upd
            _unitOfWork.Save();
            //return Json(new {succes=true, msg = $"User {state} successfully"});
            TempData["suceess"] = $"User {state} successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
