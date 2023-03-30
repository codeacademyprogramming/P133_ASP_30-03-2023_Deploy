using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;
using P133Allup.Extentions;
using P133Allup.Helpers;
using P133Allup.Models;
using P133Allup.ViewModels;

namespace P133Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageindex = 1)
        {
            IQueryable<Category> categories = _context.Categories
                .Include(c => c.Products)
                .Where(c => c.IsDeleted == false && c.IsMain);

            return View(PageNatedList<Category>.Create(categories,pageindex,5));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c=>c.Children.Where(a=>a.IsDeleted == false)).ThenInclude(c=>c.Products.Where(p => p.IsDeleted == false))
                .Include(a=>a.Products.Where(p=>p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if (await _context.Categories.AnyAsync(c=>c.IsDeleted == false && c.Name.ToLower().Contains(category.Name.Trim().ToLower())))
            {
                ModelState.AddModelError("Name", $"Bu Adda {category.Name.Trim()} Categoriya Movcuddur");
                return View(category);
            }

            if (category.IsMain)
            {
                if (category.File == null)
                {
                    ModelState.AddModelError("File", "Sekil Mecburidi");
                    return View(category);
                }

                if (!category.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "Fayl Tipi Duz Deyil");
                    return View(category);
                }

                if (!category.File.CheckFileLength(300))
                {
                    ModelState.AddModelError("File", "Fayl Olcusu Maksimum 30 kb Ola Biler");
                    return View(category);
                }

                category.Image = await category.File.CraeteFileAsync(_env,"assets","images");

                category.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Ust Categorya Mutleq Secilmelidi");
                    return View(category);
                }

                if (!await _context.Categories.AnyAsync(c=>c.IsDeleted == false && c.IsMain && c.Id == category.ParentId))
                {
                    ModelState.AddModelError("ParentId", "Duzgun Ust Categorya Sec");
                    return View(category);
                }

                category.Image = null;
            }

            category.Name = category.Name.Trim();
            category.CreatedAt= DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

             Category category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            if(id == null) return BadRequest();

            if(id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (dbCategory == null) return NotFound();

            if (category.IsMain)
            {
                if (dbCategory.IsMain )
                {
                    if (category.File != null)
                    {
                        if (category.File.CheckFileContentType("image/jpeg"))
                        {
                            ModelState.AddModelError("File", "Fayl Tipi Duz Deyil");
                            return View(category);
                        }

                        if (category.File.CheckFileLength(300))
                        {
                            ModelState.AddModelError("File", "Fayl Olcusu Maksimum 30 kb Ola Biler");
                            return View(category);
                        }

                        FileHelper.DeleteFile(dbCategory.Image, _env, "assets", "images");

                        dbCategory.Image = await category.File.CraeteFileAsync(_env, "assets", "images");
                    }
                }
                else
                {
                    if (category.File == null)
                    {
                        ModelState.AddModelError("File", "Sekil Mecburidi");
                        return View(category);
                    }

                    if (category.File.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("File", "Fayl Tipi Duz Deyil");
                        return View(category);
                    }

                    if (category.File.CheckFileLength(300))
                    {
                        ModelState.AddModelError("File", "Fayl Olcusu Maksimum 30 kb Ola Biler");
                        return View(category);
                    }

                    dbCategory.Image = await category.File.CraeteFileAsync(_env, "assets", "images");
                }

                dbCategory.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Ust Categorya Mutleq Secilmelidi");
                    return View(category);
                }

                if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.IsMain && c.Id == category.ParentId))
                {
                    ModelState.AddModelError("ParentId", "Duzgun Ust Categorya Sec");
                    return View(category);
                }

                if (dbCategory.Id == category.ParentId)
                {
                    ModelState.AddModelError("ParentId", "Eyni Ola Bilmez");
                    return View(category);
                }
                FileHelper.DeleteFile(dbCategory.Image, _env, "assets", "images");

                dbCategory.Image = null;
                dbCategory.ParentId = category.ParentId;
            }

            dbCategory.Name = category.Name.Trim();
            dbCategory.IsMain = category.IsMain;
            dbCategory.UpdatedBy = "System";
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c=>c.Children.Where(a=>a.IsDeleted == false))
                .Include(c=>c.Products.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(a => a.IsDeleted == false))
                .Include(c => c.Products.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            category.IsDeleted = true;
            category.DeletedAt= DateTime.UtcNow.AddHours(4);
            category.DeletedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
