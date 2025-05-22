using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_Application.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required, StringLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        // Navigation
        public int? SellerId { get; set; }

        //[BindNever]
        [ValidateNever]
        public User Seller { get; set; }

        public ICollection<ProductImage> Images { get; set; }

        //[BindNever]
        [ValidateNever]
        public ICollection<Review> Reviews { get; set; }
    }
}
