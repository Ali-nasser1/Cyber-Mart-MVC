using System.Security.Claims;
using CyberMart.BusinessLogic.Interfaces;
using CyberMart.DataAccess.Models;
using CyberMart.Utilities;
using CyberMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using X.PagedList;

namespace CyberMart.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            var PageNumber = page ?? 1;
            var PageSize = 4;
            var products = unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
            return View(products);
        }

        public IActionResult Details(int ProductId) 
        {
            ShoppingCart obj = new ShoppingCart()
            {
                ProductId = ProductId,
                Product = unitOfWork.Product.GetFirstElement(P => P.Id == ProductId, "Category"),
                Count = 1
            };
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart) 
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;
            ShoppingCart cart = unitOfWork.ShoppingCart.GetFirstElement(
                U => U.ApplicationUserId == claim.Value && U.ProductId == shoppingCart.ProductId
                );

            if(cart == null )
            {
                unitOfWork.ShoppingCart.Add(shoppingCart);
                unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                    unitOfWork.ShoppingCart.GetAll( U => U.ApplicationUserId == claim.Value).ToList().Count
                    );
            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(cart, shoppingCart.Count);
                unitOfWork.Complete();
            }
            return RedirectToAction("Index");
        }
    }
}
