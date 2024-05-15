using EcommerceWebAppProject.DB.Data;
using EcommerceWebAppProject.Models;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.DB.Dbinitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly AppDbContext _appDbContext;

        public DbInitializer(
            UserManager<AppUser> userManager,
            RoleManager<UserRole> roleManager,
            AppDbContext appDbContext
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
        }

        public void Initialize()
        {
            // apply pending migration
            try
            {
                if (_appDbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _appDbContext.Database.Migrate();
                }
            }
            catch (Exception ex )
            {
            }

            // create roles
            // If role admin not exist, then create all other role
            if (_roleManager.RoleExistsAsync(RoleConstant.Role_Admin).Result == false)
            {
                 _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Admin));
                 _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Sale_Employee));
                 _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Delivery_Employee));
                 _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Customer));

                // creat admin user                 
                _userManager.CreateAsync(new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Admin",
                    PhoneNumber = "1234567890"
                }, "admin@123").GetAwaiter().GetResult();
                AppUser? user = _userManager.Users.FirstOrDefault(u => u.Email == "admin@gmail.com");
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, RoleConstant.Role_Admin).GetAwaiter().GetResult();
                }

                return;
            }
        }
    }
}
