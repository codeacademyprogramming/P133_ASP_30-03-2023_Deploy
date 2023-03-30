using Microsoft.AspNetCore.Mvc;

namespace P133FirstApi.Controllers.V1
{
    [ApiController]
    [Route("/api/v1/p133")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        [Route("{id?}")]
        public IActionResult Get(int? id)
        {
            return Ok(id);
        }

        [HttpGet]
        [Route("getbyid/{id?}")]
        public IActionResult Get(int? id, string n)
        {
            return Ok(id);
        }
    }
}
