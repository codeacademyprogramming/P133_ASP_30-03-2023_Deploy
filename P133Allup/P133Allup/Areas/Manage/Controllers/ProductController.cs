using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;
using P133Allup.Extentions;
using P133Allup.Helpers;
using P133Allup.Models;
using P133Allup.ViewModels;

namespace P133Allup.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Product> products = _context.Products.Where(p=>p.IsDeleted == false);

            return View(PageNatedList<Product>.Create(products,pageIndex, 3));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _context.Brands.Where(b=>b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories
                .Include(b=>b.Children.Where(c=>c.IsDeleted == false))
                .Where(b => b.IsDeleted == false && b.IsMain)
                .ToListAsync();

            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories
                .Include(b => b.Children.Where(c => c.IsDeleted == false))
                .Where(b => b.IsDeleted == false && b.IsMain)
                .ToListAsync();

            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View(product);

            if(!await _context.Brands.AnyAsync(b=>b.IsDeleted ==false && b.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", $"Daxil Olunan Brand Id {product.BrandId} Yanlisdir");
                return View(product);
            }

            if (!await _context.Categories.AnyAsync(b => b.IsDeleted == false && b.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", $"Daxil Olunan Category Id {product.CategoryId} Yanlisdir");
                return View(product);
            }

            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(b => b.IsDeleted == false && b.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"Daxil Olunan Tag Id {tagId} Yanlisdir");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productTags.Add(productTag);
                }

                product.ProductTags = productTags;
            }
            else
            {
                ModelState.AddModelError("TagIds", "Tag Mutleq Secilmelidir");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz JPG Olmalidir");
                    return View(product);
                }

                if (!product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz 300 kb Olmalidir");
                    return View(product);
                }

                product.MainImage = await product.MainFile.CraeteFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("MainFile", "Main File Mutleq Daxil Olmalidir");
                return View(product);
            }

            if (product.HoverFile != null)
            {
                if (!product.HoverFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("HoverFile", "Hover File Yalniz JPG Olmalidir");
                    return View(product);
                }

                if (!product.HoverFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("HoverFile", "Hover File Yalniz 300 kb Olmalidir");
                    return View(product);
                }

                product.HoverImage = await product.HoverFile.CraeteFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("HoverFile", "Main File Mutleq Daxil Olmalidir");
                return View(product);
            }

            if (product.Files == null)
            {
                ModelState.AddModelError("Files", "Sekil Mutleq Secilmelidir");
                return View(product);
            }

            if (product.Files.Count() > 6)
            {
                ModelState.AddModelError("Files", "Maksimum 6 Sekil Yukleye Bilersiniz");
                return View(product);
            }

            if (product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (!file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz JPG Olmalidir");
                        return View(product);
                    }

                    if (!file.CheckFileLength(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz 300 kb Olmalidir");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CraeteFileAsync(_env, "assets", "images", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);
                }

                product.ProductImages = productImages;

            }

            string code = "";
            code = code + _context.Brands.FirstOrDefault(c => c.Id == product.BrandId).Name.Substring(0, 2);
            code = code + _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0, 2);

            product.Seria = code.ToLower().Trim();
            product.Code = _context.Products.Where(p => p.Seria == product.Seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                _context.Products.Where(p => p.Seria == product.Seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p=> p.ProductImages.Where(pi=>pi.IsDeleted == false))
                .Include(p=>p.ProductTags.Where(pt=>pt.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if(product == null) return NotFound();

            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories
                .Include(b => b.Children.Where(c => c.IsDeleted == false))
                .Where(b => b.IsDeleted == false && b.IsMain)
                .ToListAsync();

            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            product.TagIds = product.ProductTags != null && product.ProductTags.Count() > 0 ?
                product.ProductTags.Select(x => (byte)x.TagId).ToList() : new List<byte>();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Brands = await _context.Brands.Where(b => b.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories
                .Include(b => b.Children.Where(c => c.IsDeleted == false))
                .Where(b => b.IsDeleted == false && b.IsMain)
                .ToListAsync();

            ViewBag.Tags = await _context.Tags.Where(t => t.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id == null) return BadRequest();

            if(id != product.Id) return BadRequest();

            Product dbProduct = await _context.Products
                .Include(p=>p.ProductImages.Where(pi=>pi.IsDeleted == false))
                .Include(p=>p.ProductTags.Where(pt=>pt.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if(dbProduct == null) return NotFound();

            int canUpload = 6 - dbProduct.ProductImages.Count();

            if(product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} Qeder Sekil Yukleye Bilersiniz");
                return View(product);
            }

            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (!file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz JPG Olmalidir");
                        return View(product);
                    }

                    if (!file.CheckFileLength(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} Yalniz 300 kb Olmalidir");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CraeteFileAsync(_env, "assets", "images", "product"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);
                }

                dbProduct.ProductImages.AddRange(productImages);
            }

            if (product.MainFile != null)
            {
                if (!product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz JPG Olmalidir");
                    return View(product);
                }

                if (!product.MainFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz 300 kb Olmalidir");
                    return View(product);
                }

                FileHelper.DeleteFile(dbProduct.MainImage, _env, "assets", "images", "product");

                dbProduct.MainImage = await product.MainFile.CraeteFileAsync(_env, "assets", "images", "product");
            }

            if (product.HoverFile != null)
            {
                if (!product.HoverFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("HoverFile", "Hover File Yalniz JPG Olmalidir");
                    return View(product);
                }

                if (!product.HoverFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("HoverFile", "Hover File Yalniz 300 kb Olmalidir");
                    return View(product);
                }

                FileHelper.DeleteFile(dbProduct.HoverImage, _env, "assets", "images", "product");

                dbProduct.HoverImage = await product.HoverFile.CraeteFileAsync(_env, "assets", "images", "product");
            }

            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                _context.ProductTags.RemoveRange(dbProduct.ProductTags);

                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(b => b.IsDeleted == false && b.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"Daxil Olunan Tag Id {tagId} Yanlisdir");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productTags.Add(productTag);
                }

                dbProduct.ProductTags = productTags;
            }
            else
            {
                ModelState.AddModelError("TagIds", "Tag Mutleq Secilmelidir");
                return View(product);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {
            if (id == null) return BadRequest();

            if(imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p=>p.ProductImages.Where(pi=>pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            if (product.ProductImages?.Count() <= 1)
            {
                return BadRequest();
            }

            if(!product.ProductImages.Any(p=>p.Id == imageId)) return BadRequest();

            product.ProductImages.FirstOrDefault(p=>p.Id == imageId).IsDeleted = true;

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(product.ProductImages.FirstOrDefault(p => p.Id == imageId).Image, _env, "assets", "images", "product");

            List<ProductImage> productImages = product.ProductImages.Where(pi => pi.IsDeleted == false).ToList();

            return PartialView("_ProductImagePartial", productImages);
        }
    }
}
