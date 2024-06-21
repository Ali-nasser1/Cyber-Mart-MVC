using System.Security.Claims;
using CyberMart.BusinessLogic.Interfaces;
using CyberMart.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace CyberMart.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionKey) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
                HttpContext.Session.SetInt32(SD.SessionKey, unitOfWork.ShoppingCart.GetAll(X => X.ApplicationUserId == claim.Value).ToList().Count());
                return View(HttpContext.Session.GetInt32(SD.SessionKey));
            }
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}
