using CyberMart.BusinessLogic.Interfaces;
using CyberMart.DataAccess.Models;
using CyberMart.Utilities;
using CyberMart.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CyberMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        [BindProperty]
        public OrderViewModel orderViewModel { get; set; }
        public OrderController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = unitOfWork.OrderHeader.GetAll(IncludeWord: "ApplicationUser");
            return Json(new {data = orderHeaders});
        }

        public IActionResult Details(int orderid)
        {
            OrderViewModel orderViewModel = new OrderViewModel()
            {
                OrderHeader = unitOfWork.OrderHeader.GetFirstElement(O => O.Id == orderid, "ApplicationUser"),
                OrdersDetails = unitOfWork.OrderDetails.GetAll(O => O.OrderHeaderId == orderid, "Product")

            };
            return View(orderViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var order = unitOfWork.OrderHeader.GetFirstElement(O => O.Id == orderViewModel.OrderHeader.Id);
            if (order == null) return NotFound();
            order.Name = orderViewModel.OrderHeader.Name;
            order.Phone = orderViewModel.OrderHeader.Phone;
            order.Address = orderViewModel.OrderHeader.Address;
            order.City = orderViewModel.OrderHeader.City;
            if(orderViewModel.OrderHeader.Carrier != null)
            {
                order.Carrier = orderViewModel.OrderHeader.Carrier;
            }
            if (orderViewModel.OrderHeader.TrackingNumber != null)
            {
                order.Carrier = orderViewModel.OrderHeader.TrackingNumber;
            }
            unitOfWork.OrderHeader.Update(order);
            unitOfWork.Complete();

            TempData["Edit"] = "Data has been updated successfully";
            return RedirectToAction("details", "Order", new { orderid = order.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Proccess()
        {
            unitOfWork.OrderHeader.UpdateOrderStatus(orderViewModel.OrderHeader.Id, SD.Proccessing, null);
            unitOfWork.Complete();
            TempData["Edit"] = "Order status has been updated successfully";
            return RedirectToAction("details", "Order", new { orderid = orderViewModel.OrderHeader.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Fulfill()
        {
            var order = unitOfWork.OrderHeader.GetFirstElement(O => O.Id == orderViewModel.OrderHeader.Id);
            order.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
            order.Carrier = orderViewModel.OrderHeader.Carrier;
            order.OrderStatus = SD.Shipped;
            order.ShippingDate = DateTime.Now;

            unitOfWork.OrderHeader.Update(order);
            unitOfWork.Complete();
            TempData["Edit"] = "Order has shipped successfully";
            return RedirectToAction("details", "Order", new { orderid = orderViewModel.OrderHeader.Id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel()
        {
            var order = unitOfWork.OrderHeader.GetFirstElement(O => O.Id == orderViewModel.OrderHeader.Id);
            if(order.OrderStatus == SD.Approve)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntenId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                unitOfWork.OrderHeader.UpdateOrderStatus(order.Id, SD.Canceled, SD.Refund);
            }
            else
            {
                unitOfWork.OrderHeader.UpdateOrderStatus(order.Id, SD.Canceled, SD.Canceled);
            }

            unitOfWork.Complete();
            TempData["Edit"] = "Order  has been cancelled successfully";
            return RedirectToAction("details", "Order", new { orderid = orderViewModel.OrderHeader.Id });
        }
    }
}
