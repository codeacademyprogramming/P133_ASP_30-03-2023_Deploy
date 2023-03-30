using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace P133FirstApi.Controllers.V3
{
    [Route("api/v3/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        [Route("{id?}/{name}")]
        public IActionResult Get(int? id,string name)
        {
            return Ok($"{id} {name}");
        }
    }
}
