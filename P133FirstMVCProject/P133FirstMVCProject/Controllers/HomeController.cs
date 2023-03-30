using Microsoft.AspNetCore.Mvc;
using P133FirstMVCProject.Models;

namespace P133FirstMVCProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //ViewData["Name"] = "Hamid";
            //ViewData["SurName"] = "Mammadov";

            //ViewBag.Name = 32;



            //TempData["Name"] = "Hamid";

            Student student1 = new Student
            {
                Name = "Hamid",
                SurName = "Mammadov"
            };

            Student student2 = new Student
            {
                Name = "Semed",
                SurName = "Aliyev"
            };

            Student student3 = new Student
            {
                Name = "Ramiz",
                SurName = "Quliyev"
            };

            List<Student> students = new List<Student>();
            students.Add(student1);
            students.Add(student2);
            students.Add(student3);

            //ViewBag.Stu = student;

            return View(students);

            //return RedirectToAction("Test");

            //return Content("P133 Content Method");
            //return Json("P133 Content Method");
            //return View("P133 Content Method");
        }

        public IActionResult Test()
        {
            return View();
        }


        //public IActionResult Index(int id)
        //{
        //    if (id > 0)
        //    {
        //        ViewResult viewResult = new ViewResult();
        //        viewResult.ViewName = "testview";
        //        return viewResult;
        //    }
        //    else
        //    {
        //        JsonResult jsonResult = new JsonResult("Name:Hamid,SurName:Mammadov");
        //        return jsonResult;
        //    }
            
        //}

        //public JsonResult Index()
        //{
        //    JsonResult jsonResult = new JsonResult("Name:Hamid,SurName:Mammadov");

        //    return jsonResult;
        //}

        //public ContentResult Index()
        //{
        //    ContentResult contentResult = new ContentResult();
        //    contentResult.Content = "Hello P133";
        //    contentResult.ContentType = "application/json";
        //    contentResult.StatusCode = 200;
        //    return contentResult;
        //}
    }
}
