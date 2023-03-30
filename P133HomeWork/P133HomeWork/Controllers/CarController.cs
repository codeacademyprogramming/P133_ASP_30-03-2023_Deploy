using Microsoft.AspNetCore.Mvc;
using P133HomeWork.Models;

namespace P133HomeWork.Controllers
{
    public class CarController : Controller
    {
        private readonly List<Car> _cars;

        public CarController()
        {
            _cars = new List<Car>
            {
                new Car{Id = 1, Engine = "E-1", ModelId = 1,Name = "N-1",Year=2022},
                new Car{Id = 2, Engine = "E-2", ModelId = 1,Name = "N-2",Year=2022},
                new Car{Id = 3, Engine = "E-3", ModelId = 2,Name = "N-3",Year=2022},
                new Car{Id = 4, Engine = "E-4", ModelId = 2,Name = "N-4",Year=2022},
                new Car{Id = 5, Engine = "E-5", ModelId = 3,Name = "N-5",Year=2022},
                new Car{Id = 6, Engine = "E-6", ModelId = 3,Name = "N-6",Year=2022},
                new Car{Id = 7, Engine = "E-7", ModelId = 4,Name = "N-7",Year=2022},
                new Car{Id = 8, Engine = "E-8", ModelId = 4,Name = "N-8",Year=2022},
            };
        }

        public IActionResult Index(int? id)
        {
            if (id == null)
            {
                return View(_cars);
            }

            if (_cars.Exists(c=>c.ModelId == id))
            {
                return View(_cars.FindAll(c => c.ModelId == id));
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return BadRequest("Id Null Ola Bilmez");

            if (!_cars.Exists(c => c.Id == id)) return NotFound("Gonderilen Id Yanlis");

            Car car = _cars.Find(c => c.Id == id);

            return View(car);
        }
    }
}
