using E_Commerce_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Web_Application.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }


        #region User APIs

        // Get All User
        public async Task<IActionResult> Users()
        {
            var users = await _db.Users.ToListAsync();
            return View(users);
        }

        #endregion

    }
}
