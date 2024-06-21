using AutoMapper;
using CyberMart.BusinessLogic.Interfaces;
using CyberMart.DataAccess.Models;
using CyberMart.Utilities;
using CyberMart.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace CyberMart.Areas.Admin.Controllers
{
	[Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
		private readonly IMapper mapper;

        //private readonly IWebHostEnvironment webHostEnvironment

        public ProductController(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        
        public IActionResult Index()
        {
            var Products = unitOfWork.Product.GetAll(IncludeWord: "Category"); // 
            var MappedProduct = mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(Products);
            return View(MappedProduct);
        }
        public IActionResult GetData()
        {
            var Products = unitOfWork.Product.GetAll(IncludeWord: "Category"); // 
			var TotalRecords = Products.Count();
            var JsonFile = new {data = Products };
			return Ok(JsonFile);
        }
		public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                productVM.ImageName = DocumentSettings.UploadFile(productVM.Image, "Products");
				var MappedProduct = mapper.Map<ProductViewModel, Product>(productVM);
                unitOfWork.Product.Add(MappedProduct);
                unitOfWork.Complete();
                TempData["Create"] = "Data has been created succesfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();
            var product = unitOfWork.Product.GetFirstElement(C => C.Id == id, "Category");
            if(product == null) return NotFound();
            var MappedProduct = mapper.Map<Product, ProductViewModel>(product);
            TempData["ImageName"] = MappedProduct.ImageName;
            return View(MappedProduct);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                product.ImageName = (string)TempData["ImageName"];
                if(product.Image != null)
                {
                    DocumentSettings.DeleteFile(product.ImageName, "Products");
                    product.ImageName = DocumentSettings.UploadFile(product.Image, "Products");
                }
                var MappedProduct = mapper.Map<ProductViewModel, Product>(product);
                unitOfWork.Product.Update(MappedProduct);
                unitOfWork.Complete();
                TempData["Edit"] = "Data has been Updated succesfully";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = unitOfWork.Product.GetFirstElement(X => X.Id == id);
            if(product == null)
                return Json(new {success = false, message = "Error while deleting the product"});
            unitOfWork.Product.Remove(product);
            DocumentSettings.DeleteFile(product.ImageName,"Products");
            unitOfWork.Complete();
            return Json(new { success = true, message = "product has been deleted succesfully" });
        }
    }

}
