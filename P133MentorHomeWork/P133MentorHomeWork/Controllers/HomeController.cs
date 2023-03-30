using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133MentorHomeWork.DataAccessLayer;
using P133MentorHomeWork.Models;

namespace P133MentorHomeWork.Controllers
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
            //ViewBag.Index = 1;

            IEnumerable<Feature> features = await _context.Features.ToListAsync();
            return View(features);
        }
    }
}
