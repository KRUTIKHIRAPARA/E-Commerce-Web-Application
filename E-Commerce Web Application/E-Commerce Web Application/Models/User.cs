using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Web_Application.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required,StringLength(100)]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string PasswordHash { get; set; }  // store hashed password

        [Required]
        public string Role { get; set; }  // Admin, Seller, Buyer

        public string FullName { get; set; }
    }
}
