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

        public async void Initialize()
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
            if (!await _roleManager.RoleExistsAsync(RoleConstant.Role_Admin))
            {
                await _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Admin));
                await _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Sale_Employee));
                await _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Delivery_Employee));
                await _roleManager.CreateAsync(new UserRole(RoleConstant.Role_Customer));
                
                var createdUser = await _userManager.CreateAsync(
                    new AppUser
                    {
                        UserName = "admin@gmail.com",
                        Email = "admin@gmail.com",
                        Name = "Admin",
                        PhoneNumber = "1234567890",
                        EmailConfirmed = true
                    }, "Admin@123");

                if( createdUser.Succeeded) {
                    var user = await _userManager.FindByEmailAsync("admin@gmail.com");
                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, RoleConstant.Role_Admin);
                    }
                }
                _appDbContext.SaveChanges();
                return;
            }
        }
    }
}
