using Microsoft.AspNetCore.Mvc;

namespace P133FirstApi.Controllers.V2
{
    [ApiController]
    [Route("/api/v2/categories")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        [Route("getbyid/{id?}")]
        public IActionResult Get(int? id)
        {
            id += 5;
            return Ok(id);
        }
    }
}
