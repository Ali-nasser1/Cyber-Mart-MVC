﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CyberMart.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CyberMart.DataAccess.Models
{
	public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1, 100, ErrorMessage = "you must enter a value between 1 : 100")]
        public int Count {  get; set; }

        public string ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser ApplicationUser { get; set; }
	}
}
