using Microsoft.AspNetCore.Mvc;

namespace P133StartBootsstrap.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
