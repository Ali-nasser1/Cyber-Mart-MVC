using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CyberMart.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CyberMart.ViewModels
{
	public class ProductViewModel
	{
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[DisplayName("Image")]
		[ValidateNever]
		public string ImageName { get; set; }
		[ValidateNever]
		public IFormFile? Image { get; set; }
		[Required]
		public decimal Price { get; set; }
		[Required]
		[DisplayName("Category")]
		[ForeignKey("Category")]
		public int CategoryId { get; set; }
		[ValidateNever]
		public Category Category { get; set; }
	}
}
