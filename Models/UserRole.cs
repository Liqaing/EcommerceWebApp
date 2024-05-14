using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Models
{
    public class UserRole : IdentityRole
    {
        public UserRole() : base() { }

        public UserRole(string roleName) : base(roleName) { }
    }
}
