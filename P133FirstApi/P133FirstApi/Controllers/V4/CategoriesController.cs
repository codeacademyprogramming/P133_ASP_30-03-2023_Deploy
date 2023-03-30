using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133FirstApi.DataAccessLayer;
using P133FirstApi.DTOs.CategorDTOs;
using P133FirstApi.Entities;

namespace P133FirstApi.Controllers.V4
{
    /// <summary>
    /// Categories Services
    /// </summary>
    [Route("api/v4/[controller]")]
    [ApiController]
    [Authorize(Roles ="Member")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates an Employee.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/v4/categories
        ///     {        
        ///       "name": "Test"      
        ///     }
        /// </remarks>
        /// <param name="category"></param>
        /// <returns>A newly created category Id</returns>
        /// <response code="400">Object Invalid</response>
        /// <response code="409">Name Already Exist</response>          
        /// <response code="201">Successfully Created</response>          
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(201)]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> Post(/*[FromForm]*/CategoryPostDto categoryPostDto)
        {
            //if(!ModelState.IsValid) return BadRequest(ModelState);

            //if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryPostDto.Name.Trim().ToLower()))
            //{
            //    return Conflict($"{categoryPostDto.Name} Already Exist");
            //}

            //Category category = new Category
            //{
            //    Name = categoryPostDto.Name.Trim(),
            //};

            Category category = _mapper.Map<Category>(categoryPostDto);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, category.Id);
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> Get(/*[FromRoute]*/int? id)
        {
            if(id == null) return BadRequest("Id Is Null");

            Category category = await _context.Categories
                .Include(c=>c.Products)
                .FirstOrDefaultAsync(c=>c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound("Id Is InCorrect");

            //return Ok(new CategoryGetDto
            //{
            //    Id = category.Id,
            //    Name = category.Name
            //});

            CategoryGetDto categoryGetDto = _mapper.Map<CategoryGetDto>(category);

            return Ok(categoryGetDto);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync());
        }

        [HttpPut]
        public async Task<IActionResult> Put(CategoryPutDto categoryPutDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == categoryPutDto.Id && c.IsDeleted == false);

            if (dbCategory == null) return NotFound("Id Is InCorrect");

            if (await _context.Categories.AnyAsync(c=>c.Name.ToLower() == categoryPutDto.Name.Trim().ToLower() && c.Id != categoryPutDto.Id))
            {
                return Conflict($"{categoryPutDto.Name} Already Exist");
            }

            dbCategory.Name = categoryPutDto.Name.Trim();
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        [Route("{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest("Id Is Null");

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound("Id Is InCorrect");

            category.IsDeleted = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
