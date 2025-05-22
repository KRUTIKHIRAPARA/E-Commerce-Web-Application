using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_Application.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
