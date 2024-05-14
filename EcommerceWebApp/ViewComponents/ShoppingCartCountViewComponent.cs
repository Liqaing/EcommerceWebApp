using EcommerceWebAppProject.DB.Repository.IRepository;
using EcommerceWebAppProject.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Diagnostics;

namespace EcommerceWebAppProject.EcommerceWebApp.ViewComponents
{
    public class ShoppingCartCountViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartCountViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIndentiy = (ClaimsIdentity) User.Identity;
            var claim = claimsIndentiy.FindFirst(ClaimTypes.NameIdentifier);

            int cartCount = 0;

            if (claim != null)
            {
                cartCount = _unitOfWork.ShoppingCart.GetAll(u => u.appUserId == claim.Value &&
                    u.shoppingCartStatus == ShoppingCartStatusConstant.StatusActive).Count();
                HttpContext.Session.SetInt32(OrderAndPaymentStatusConstate.SessionCart, cartCount);                
            }
            else
            {
                HttpContext.Session.Clear();
            }
            return View(cartCount);
        }

    }
}
