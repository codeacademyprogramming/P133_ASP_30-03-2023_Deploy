using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;
using P133Allup.Models;
using P133Allup.ViewModels;
using P133Allup.ViewModels.ProductViewsModels;

namespace P133Allup.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Product> products = _context.Products.Where(p=>p.IsDeleted == false);

            return View(PageNatedList<Product>.Create(products,pageIndex,12));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Brand)
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false)).ThenInclude(pt => pt.Tag)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            ProductReviewVM productReviewVM = new ProductReviewVM
            {
                Product = product,
                Review = new Review { ProductId= id },
            };

            return View(productReviewVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> AddReviuw(Review review)
        {
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Brand)
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false)).ThenInclude(pt => pt.Tag)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == review.ProductId);

            ProductReviewVM productReviewVM = new ProductReviewVM { Product = product, Review = review };

            if (!ModelState.IsValid) return View("Detail", productReviewVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (product.Reviews != null && product.Reviews.Count() > 0 && product.Reviews.Any(r=>r.UserId == appUser.Id))
            {
                ModelState.AddModelError("Name", "Siz Artiq Fikir Bildirmisiz");
                return View("Detail", productReviewVM);
            }

            review.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            review.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail),new { id = product.Id});
        }

        public async Task<IActionResult> ProductModal(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return PartialView("_ModalPartial", product);
        }

        public async Task<IActionResult> Search(int? categoryId, string search)
        {
            if (categoryId != null && categoryId > 0)
            {
                if (!await _context.Categories.AnyAsync(c => c.Id == categoryId))
                {
                    return BadRequest();
                }
            }

            IEnumerable<Product> products = await _context.Products
                   .Where(p => p.IsDeleted == false && categoryId != null && categoryId > 0 ? p.CategoryId == categoryId : true &&
                   (p.Title.ToLower().Contains(search.ToLower()) || p.Brand.Name.ToLower().Contains(search.ToLower()))).ToListAsync();

            return PartialView("_SearchPartial", products);
        }

        //public async Task<IActionResult> ProductModal(int? id)
        //{
        //    Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

        //    return Json(product);
        //}
    }
}
