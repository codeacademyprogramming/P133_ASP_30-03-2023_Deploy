using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;
using P133Allup.ViewModels.HomeViewModels;
using System.Text;

namespace P133Allup.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM
            {
                Sliders = await _context.Sliders.Where(s=>s.IsDeleted == false).ToListAsync(),
                Categories = await _context.Categories.Where(c=>c.IsDeleted == false && c.IsMain).ToListAsync(),
                Features = await _context.Products.Where(p=>p.IsDeleted == false && p.IsFeatured).ToListAsync(),
                BestSeller = await _context.Products.Where(p => p.IsDeleted == false && p.IsBestSeller).ToListAsync(),
                NewArrival = await _context.Products.Where(p => p.IsDeleted == false && p.IsNewArrival).ToListAsync()

            };

            return View(vm);
        }

        //public async Task<IActionResult> SetSession()
        //{
        //    HttpContext.Session.SetString("P133", "First Session Data");

        //    return Content("Session Elave Olundu");
        //}

        //public async Task<IActionResult> GetSession()
        //{
        //    var ses = HttpContext.Session.GetString("P133");

        //    return Content(ses);
        //}

        //public async Task<IActionResult> SetCookie()
        //{
        //    HttpContext.Response.Cookies.Append("P133", "First Cookie Data");

        //    return Content("Cookie Elave olundu");
        //}

        //public async Task<IActionResult> GetCookie()
        //{
        //    string a = HttpContext.Request.Cookies["P133"];

        //    return Content(a);
        //}
    }
}
