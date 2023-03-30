using Microsoft.AspNetCore.Mvc;

namespace P133StartBootsstrap.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
