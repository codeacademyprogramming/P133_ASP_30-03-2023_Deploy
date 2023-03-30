using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133MentorHomeWork.DataAccessLayer;
using P133MentorHomeWork.Models;
using P133MentorHomeWork.ViewModels.PricingVM;

namespace P133MentorHomeWork.Controllers
{
    public class PricingController : Controller
    {
        private readonly AppDbContext _context;

        public PricingController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            //ViewBag.Index = 2;

            IEnumerable<Pricing> pricings = await _context.Pricings.Include(p=>p.PricingServices).ToListAsync();
            IEnumerable<Service> services = await _context.Services.ToListAsync();

            PricingVM pricingVM = new PricingVM
            {
                Pricings = pricings,
                Services = services
            };

            return View(pricingVM);
        }
    }
}
