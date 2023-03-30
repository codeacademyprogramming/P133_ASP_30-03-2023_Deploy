using Microsoft.AspNetCore.Mvc;
using P133HomeWork.Models;

namespace P133HomeWork.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Marka> markas = new List<Marka>
            {
                new Marka{Id=1,Name="Mercedes"},
                new Marka{Id=2,Name="BMW"},
            };

            return View(markas);
        }
    }
}
