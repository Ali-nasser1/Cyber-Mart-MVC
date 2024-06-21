using CyberMart.BusinessLogic.Interfaces;
using CyberMart.DataAccess.Contexts;
using CyberMart.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;

namespace CyberMart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = unitOfWork.Category.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(category);
                unitOfWork.Complete();
                TempData["Create"] = "Data has been created succesfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var category = unitOfWork.Category.GetFirstElement(C => C.Id == id);
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(category);
                unitOfWork.Complete();
                TempData["Edit"] = "Data has been Updated succesfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
		[HttpGet]
		public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var category = unitOfWork.Category.GetFirstElement(C => C.Id == id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Category category)
        {
            if (ModelState.IsValid)
            {

                unitOfWork.Category.Remove(category);
                unitOfWork.Complete();
                TempData["Delete"] = "Data has been deleted succesfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }

}
