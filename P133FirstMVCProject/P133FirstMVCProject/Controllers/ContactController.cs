using Microsoft.AspNetCore.Mvc;

namespace P133FirstMVCProject.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
