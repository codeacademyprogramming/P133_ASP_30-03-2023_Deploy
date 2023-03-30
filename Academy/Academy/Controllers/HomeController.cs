using Academy.Models;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<Group> groups = new List<Group>
            {
                new Group{Id=1,GroupNo = "P133"},
                new Group{Id=2,GroupNo = "P229"},
                new Group{Id=3,GroupNo = "P228"},
            };


            return View(groups);
        }
    }
}
