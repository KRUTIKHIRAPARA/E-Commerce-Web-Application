using E_Commerce_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

namespace E_Commerce_Web_Application.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }


        #region User Login and Register APIs

        public IActionResult Register() => View();

        [HttpPost]
        [HttpPost]
        public IActionResult Register(string fullName, string email, string password, string role)
        {
            if (string.IsNullOrEmpty(role) || !(new[] { "Admin", "Seller", "Buyer" }.Contains(role)))
            {
                ModelState.AddModelError("role", "Please select a valid role.");
                return View();
            }

            if (_db.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "Email already exists.");
                return View();
            }

            var hashedPassword = HashPassword(password);
            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = hashedPassword,
                Role = role,
                UserName = fullName,
            };

            _db.Users.Add(newUser);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var hashedPassword = HashPassword(password);
            var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower() && u.PasswordHash == hashedPassword);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserRole", user.Role);
                return RedirectToAction("Index", "Product");
            }
            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _db.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(User model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _db.Users.Find(userId);
            if (user != null)
            {
                user.FullName = model.FullName;
                _db.SaveChanges();
                ViewBag.Message = "Profile updated successfully.";
            }
            return View(user);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        #endregion
    }
}
