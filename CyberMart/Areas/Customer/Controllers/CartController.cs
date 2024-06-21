using System.Numerics;
using System.Security.Claims;
using CyberMart.BusinessLogic.Interfaces;
using CyberMart.BusinessLogic.Repositories;
using CyberMart.DataAccess.Models;
using CyberMart.Utilities;
using CyberMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace CyberMart.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartViewModel shoppingCartViewModel { get; set; }
        public int TotalCart { get; set; }
        public CartController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCartViewModel = new ShoppingCartViewModel()
			{
				CartsList = unitOfWork.ShoppingCart.GetAll(U => U.ApplicationUserId == claim.Value, "Product"),
				OrderHeader = new()
			};

            foreach (var item in shoppingCartViewModel.CartsList)
            {
                shoppingCartViewModel.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }
            return View(shoppingCartViewModel);
        }

        public IActionResult Plus(int cartId)
        {
            var shoppingCart = unitOfWork.ShoppingCart.GetFirstElement(S => S.Id == cartId);
            unitOfWork.ShoppingCart.IncreaseCount(shoppingCart, 1);
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }

        public IActionResult Minus(int cartId)
        {
			var shoppingCart = unitOfWork.ShoppingCart.GetFirstElement(S => S.Id == cartId);
            if(shoppingCart.Count > 1)
            {
			   unitOfWork.ShoppingCart.DecreaseCount(shoppingCart, 1);
            }
            else
            {
                unitOfWork.ShoppingCart.Remove(shoppingCart);
			    var count = unitOfWork.ShoppingCart.GetAll(U => U.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count() - 1;
				HttpContext.Session.SetInt32(SD.SessionKey, count);
            }
			   unitOfWork.Complete();
			return RedirectToAction("Index");
		}

		public IActionResult Delete(int cartId)
		{
			var shoppingCart = unitOfWork.ShoppingCart.GetFirstElement(S => S.Id == cartId);
			unitOfWork.ShoppingCart.Remove(shoppingCart);
			unitOfWork.Complete();
            var count = unitOfWork.ShoppingCart.GetAll(U => U.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
            HttpContext.Session.SetInt32(SD.SessionKey, count);
            return RedirectToAction("Index");
		}
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCartViewModel = new ShoppingCartViewModel()
			{
				CartsList = unitOfWork.ShoppingCart.GetAll(U => U.ApplicationUserId == claim.Value, "Product"),
                OrderHeader = new()
			};

			// you can use utility or mapper here
			shoppingCartViewModel.OrderHeader.ApplicationUser = unitOfWork.ApplicationUser.GetFirstElement(x => x.Id == claim.Value);
			shoppingCartViewModel.OrderHeader.Name = shoppingCartViewModel.OrderHeader.ApplicationUser.Name;
			shoppingCartViewModel.OrderHeader.Address = shoppingCartViewModel.OrderHeader.ApplicationUser.Address;
			shoppingCartViewModel.OrderHeader.City = shoppingCartViewModel.OrderHeader.ApplicationUser.City;
			shoppingCartViewModel.OrderHeader.Phone = shoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;

			foreach (var item in shoppingCartViewModel.CartsList)
			{
				shoppingCartViewModel.TotalPrices += (item.Count * item.Product.Price);
			}
			return View(shoppingCartViewModel);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Summary(ShoppingCartViewModel shoppingCartViewModel)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			shoppingCartViewModel.CartsList = unitOfWork.ShoppingCart.GetAll(U => U.ApplicationUserId == claim.Value, "Product");

			shoppingCartViewModel.OrderHeader.OrderStatus = SD.Pending;
			shoppingCartViewModel.OrderHeader.PaymentStatus = SD.Pending;
			shoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
			shoppingCartViewModel.OrderHeader.ApplicationUserId = claim.Value;

			foreach (var item in shoppingCartViewModel.CartsList)
			{
				shoppingCartViewModel.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
			}

			unitOfWork.OrderHeader.Add(shoppingCartViewModel.OrderHeader);
			unitOfWork.Complete();


			foreach (var item in shoppingCartViewModel.CartsList)
			{
				OrderDetails orderDetails = new OrderDetails()
				{
					ProductId = item.ProductId,
					OrderHeaderId = shoppingCartViewModel.OrderHeader.Id,
					Price = item.Product.Price,
					Count = item.Count
				};

				unitOfWork.OrderDetails.Add(orderDetails);
				unitOfWork.Complete();
			}

			var domain = "https://localhost:44330/";
			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"customer/cart/orderconfirmation?id={shoppingCartViewModel.OrderHeader.Id}",
			    CancelUrl = domain + $"customer/cart/index"
			};

			foreach (var item in shoppingCartViewModel.CartsList)
			{
				var sessionlineoption = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Product.Price * 100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name
						},
					},
					Quantity = item.Count,
				};
				options.LineItems.Add(sessionlineoption);
			}

			var service = new SessionService();
			Session session = service.Create(options);
			shoppingCartViewModel.OrderHeader.SessionId = session.Id;
			unitOfWork.Complete();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}

		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstElement(O => O.Id == id);
			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId);

			if(session.PaymentStatus.ToLower() == "paid")
			{
				unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Approve, SD.Approve);
				orderHeader.PaymentIntenId = session.PaymentIntentId;
				unitOfWork.Complete();
			}
			List<ShoppingCart> shoppingCarts = unitOfWork.ShoppingCart.GetAll(U => U.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			unitOfWork.Complete();
			return View();
		}

	}
}
