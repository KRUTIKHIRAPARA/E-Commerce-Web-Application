using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_Application.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
