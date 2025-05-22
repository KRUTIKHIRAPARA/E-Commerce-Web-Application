using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Web_Application.Models;

namespace E_Commerce_Web_Application.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }


        #region Product APIs

        public IActionResult Index(string search, string category, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            int pageSize = 5;
            var products = _db.Products.Include(p => p.Images).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);
            if (minPrice.HasValue)
                products = products.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue)
                products = products.Where(p => p.Price <= maxPrice);

            var totalItems = products.Count();

            var result = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Search = search;
            ViewBag.Category = category;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(result);
        }

        public IActionResult Create()
        {
            if (!IsSellerOrAdmin()) return Unauthorized();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model, List<IFormFile> images)
        {
            if (!IsSellerOrAdmin()) return Unauthorized();

            if (ModelState.IsValid)
            {
                var sellerId = HttpContext.Session.GetInt32("UserId");
                if (sellerId == null) return Unauthorized();

                model.SellerId = sellerId.Value;

                _db.Products.Add(model);
                await _db.SaveChangesAsync();

                var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsRoot))
                {
                    Directory.CreateDirectory(uploadsRoot);
                }

                // Save images
                if (images != null && images.Count > 0)
                {
                    foreach (var img in images)
                    {
                        if (img.Length > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                            var filePath = Path.Combine(uploadsRoot, fileName);

                            using var stream = new FileStream(filePath, FileMode.Create);
                            await img.CopyToAsync(stream);

                            _db.ProductImages.Add(new ProductImage
                            {
                                ProductId = model.ProductId,
                                ImageUrl = "/uploads/" + fileName
                            });
                        }
                    }
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            if (!IsSellerOrAdmin()) return Unauthorized();

            var product = _db.Products.Include(p => p.Images).FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            if (!IsOwnerOrAdmin(product.SellerId.Value)) return Unauthorized();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product model, List<IFormFile> images)
        {
            if (!IsSellerOrAdmin()) return Unauthorized();

            var product = _db.Products.Include(p => p.Images).FirstOrDefault(p => p.ProductId == model.ProductId);
            if (product == null) return NotFound();

            if (!IsOwnerOrAdmin(product.SellerId.Value)) return Unauthorized();

            if (ModelState.IsValid)
            {
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Category = model.Category;
                product.StockQuantity = model.StockQuantity;

                // Add new images
                if (images != null && images.Count > 0)
                {
                    foreach (var img in images)
                    {
                        if (img.Length > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                            using var stream = new FileStream(filePath, FileMode.Create);
                            await img.CopyToAsync(stream);

                            var productImage = new ProductImage
                            {
                                ProductId = product.ProductId,
                                ImageUrl = "/uploads/" + fileName
                            };
                            _db.ProductImages.Add(productImage);
                        }
                    }
                }

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!IsSellerOrAdmin()) return Unauthorized();

            var product = _db.Products.Find(id);
            if (product == null) return NotFound();

            if (!IsOwnerOrAdmin(product.SellerId.Value)) return Unauthorized();

            _db.Products.Remove(product);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id, int productId)
        {
            var image = await _db.ProductImages.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            var imagePath = Path.Combine(_env.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _db.ProductImages.Remove(image);
            await _db.SaveChangesAsync();

            return RedirectToAction("Edit", new { id = productId });
        }

        public IActionResult Details(int id)
        {
            var product = _db.Products
                .Include(p => p.Images)
                .Include(p => p.Reviews).ThenInclude(r => r.User)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return NotFound();

            return View(product);
        }

        #endregion

        #region Private Logic Methods

        private bool IsSellerOrAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Seller" || role == "Admin";
        }

        private bool IsOwnerOrAdmin(int ownerId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");
            return (userId == ownerId) || role == "Admin";
        }

        #endregion
    }
}
