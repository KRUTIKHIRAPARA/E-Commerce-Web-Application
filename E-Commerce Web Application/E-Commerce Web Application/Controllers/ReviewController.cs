using E_Commerce_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Web_Application.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReviewController(ApplicationDbContext db)
        {
            _db = db;
        }


        #region Review APIs

        [HttpPost]
        public IActionResult AddReview(int ProductId, int Rating, string Comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            if (Rating < 1 || Rating > 5)
            {
                return BadRequest("Invalid rating value.");
            }

            var review = new Review
            {
                ProductId = ProductId,
                UserId = userId.Value,
                Rating = Rating,
                Comment = Comment,
                CreatedAt = DateTime.Now
            };

            _db.Reviews.Add(review);
            _db.SaveChanges();

            return RedirectToAction("Details", "Product", new { id = ProductId });
        }

        #endregion
    }
}
