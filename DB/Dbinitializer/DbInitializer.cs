﻿using EcommerceWebAppProject.DB.Data;
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
                    }, "Admin@1234");

                if( createdUser.Succeeded) {
                    var user = await _userManager.FindByEmailAsync("admin@gmail.com");
                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, RoleConstant.Role_Admin);
                    }
                }

                var customerUser = await _userManager.CreateAsync(
                    new AppUser
                    {
                        UserName = "customer@gmail.com",
                        Email = "customer@gmail.com",
                        Name = "Customer",
                        PhoneNumber = "123212890",
                        EmailConfirmed = true
                    }, "Customer@1234");

                if (customerUser.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync("customer@gmail.com");
                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, RoleConstant.Role_Customer);
                    }
                }


                var saleUser = await _userManager.CreateAsync(
                    new AppUser
                    {
                        UserName = "sale@gmail.com",
                        Email = "sale@gmail.com",
                        Name = "Sale",
                        PhoneNumber = "01212890",
                        EmailConfirmed = true
                    }, "Sale@1234");

                if (saleUser.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync("sale@gmail.com");
                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, RoleConstant.Role_Sale_Employee);
                    }
                }


                var deliveryUser = await _userManager.CreateAsync(
                    new AppUser
                    {
                        UserName = "delivery@gmail.com",
                        Email = "delivery@gmail.com",
                        Name = "Delivery",
                        PhoneNumber = "01212890",
                        EmailConfirmed = true
                    }, "Delivery@1234");

                if (deliveryUser.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync("delivery@gmail.com");
                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(user, RoleConstant.Role_Delivery_Employee);
                    }
                }

                _appDbContext.SaveChanges();
                return;
            }
        }
    }
}
