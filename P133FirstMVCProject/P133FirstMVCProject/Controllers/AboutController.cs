using Microsoft.AspNetCore.Mvc;

namespace P133FirstMVCProject.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
