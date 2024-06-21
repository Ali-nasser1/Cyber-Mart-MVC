using CyberMart.BusinessLogic.Interfaces;
using CyberMart.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CyberMart.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.AdminRole)]
	public class DashboardController : Controller
	{
		private readonly IUnitOfWork unitOfWork;

		public DashboardController(IUnitOfWork _unitOfWork)
        {
			unitOfWork = _unitOfWork;
		}
        public IActionResult Index()
		{
			ViewBag.AllOrders = unitOfWork.OrderHeader.GetAll().Count();
			ViewBag.ApprovedOrders = unitOfWork.OrderHeader.GetAll(O => O.OrderStatus == SD.Approve).Count();
			ViewBag.Users = unitOfWork.ApplicationUser.GetAll().Count();
			ViewBag.Products = unitOfWork.Product.GetAll().Count();
			return View();
		}
	}
}
