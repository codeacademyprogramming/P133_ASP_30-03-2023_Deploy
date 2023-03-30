using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;
using P133Allup.Models;
using P133Allup.ViewModels;
using System.Data;

namespace P133Allup.Areas.Manage.Controllers
{
    [Area("Manage")]
    //[Authorize(Roles = "SuperAdmin,Admin")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        //[AllowAnonymous]
        public async Task<IActionResult> Index(int pageIndex=1)
        {
            IQueryable<Brand> brands = _context.Brands
                .Include(b=>b.Products.Where(p=>p.IsDeleted==false))
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b=>b.Id);

            //ViewBag.TotalPage = (int)Math.Ceiling((decimal)brands.Count() / 3);
            //ViewBag.PageIndex = pageIndex;

            //brands = brands.Skip((pageIndex - 1) * 3).Take(3);

            return View(PageNatedList<Brand>.Create(brands,pageIndex,3));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.Include(b=>b.Products.Where(p=>p.IsDeleted == false)).FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return NotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            if (await _context.Brands.AnyAsync(b=>b.IsDeleted == false && b.Name.ToLower().Contains(brand.Name.Trim().ToLower())))
            {
                ModelState.AddModelError("Name", $"Bu {brand.Name} Adda Brand Artiq Movcuddur");
                return View(brand);
            }

            brand.Name = brand.Name.Trim();
            brand.CreatedBy = "System";
            brand.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return NotFound();

            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? id, Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            if (id == null) return BadRequest();

            if(id != brand.Id) return BadRequest();

            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return NotFound();

            if (await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower().Contains(brand.Name.Trim().ToLower()) && brand.Id != b.Id))
            {
                ModelState.AddModelError("Name", $"Bu {brand.Name} Adda Brand Artiq Movcuddur");
                return View(brand);
            }

            dbBrand.Name = brand.Name.Trim();
            dbBrand.UpdatedBy = "Systme";
            dbBrand.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"{dbBrand.Id} Id-li {dbBrand.Name} Ugurla Editlendi";

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id, int pageIndex = 1)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.Include(b => b.Products.Where(p => p.IsDeleted == false)).FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return NotFound();

            brand.IsDeleted = true;
            brand.DeletedBy = User.Identity.Name;
            brand.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<Brand> brands = _context.Brands
                .Include(b => b.Products.Where(p => p.IsDeleted == false))
                .Where(b => b.IsDeleted == false)
                .OrderByDescending(b => b.Id);

            return PartialView("_BrandIndexPartial",PageNatedList<Brand>.Create(brands, pageIndex,3));

            

            //return View(brand);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteBrand(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.Include(b => b.Products.Where(p => p.IsDeleted == false)).FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (brand == null) return NotFound();

            //_context.Brands.Remove(brand);

            brand.IsDeleted = true;
            brand.DeletedBy = "System";
            brand.DeletedAt = DateTime.UtcNow.AddHours(4);

            foreach (Product product in brand.Products)
            {
                //product.BrandId = null;

                //OR

                product.IsDeleted = true;
                product.DeletedBy = "System";
                product.DeletedAt = DateTime.UtcNow.AddHours(4);

            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
