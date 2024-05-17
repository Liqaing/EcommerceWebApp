using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceWebAppProject.Models.ViewModel
{
    public class UserVM
    {
        public AppUser appUser { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
