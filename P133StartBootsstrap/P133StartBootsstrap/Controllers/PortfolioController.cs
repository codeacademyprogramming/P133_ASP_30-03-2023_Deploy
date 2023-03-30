using Microsoft.AspNetCore.Mvc;

namespace P133StartBootsstrap.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
