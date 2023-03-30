using Academy.Models;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Controllers
{
    public class StudentController : Controller
    {
        private readonly List<Student> _students;

        public StudentController()
        {
            _students = new List<Student>
            {
                new Student{Id = 1, Name = "Ramiz", SurName = "Quliyev", Age = 21, GroupId = 1},
                new Student{Id = 2, Name = "Semed", SurName = "Aliyev", Age = 20, GroupId = 1},
                new Student{Id = 3, Name = "Nurlan", SurName = "Nezerov", Age = 29, GroupId = 1},
                new Student{Id = 4, Name = "Benovse", SurName = "Abdiyeva", Age = 20, GroupId = 2},
                new Student{Id = 5, Name = "Vusal", SurName = "Aliyev", Age = 18, GroupId = 2},
                new Student{Id = 6, Name = "Faiq", SurName = "Aliyev", Age = 27, GroupId = 3},
                new Student{Id = 7, Name = "Aqil", SurName = "Soltanli", Age = 18, GroupId = 3},
            };
        }

        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                return View(_students);
            }

            if (_students.Exists(s=>s.GroupId == id))
            {
                return View(_students.FindAll(s => s.GroupId == id));
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            if (_students.Exists(s => s.Id == id))
            {
                return View(_students.Find(s => s.Id == id));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
