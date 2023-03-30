using Microsoft.AspNetCore.Mvc;
using P133HomeWork.Models;
using System.Collections.Generic;
using System.Reflection;

namespace P133HomeWork.Controllers
{
    public class ModelController : Controller
    {
        private readonly List<Model> _models;

        public ModelController()
        {
            _models = new List<Model>
            {
                new Model{Id=1, Name="C160",MarkaId =1},
                new Model{Id=2, Name="E-Class",MarkaId =1},
                new Model{Id=3, Name="X5",MarkaId =2},
                new Model{Id=4, Name="X7",MarkaId =2},
            };
        }

        public IActionResult Index(int? id)
        {

            if (id == null)
            {
                return View(_models);
            }

            if (_models.Exists(m=>m.MarkaId == id))
            {
                return View(_models.FindAll(m => m.MarkaId == id));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
