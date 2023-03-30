using Microsoft.AspNetCore.Mvc;

namespace P133StartBootsstrap.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
