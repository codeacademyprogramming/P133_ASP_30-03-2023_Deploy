using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133FirstDbMigrationConnection.DataAccessLayer;
using P133FirstDbMigrationConnection.Models;

namespace P133FirstDbMigrationConnection.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groups = _context.Groups
                .Include(g => g.Students)
                .Include(g => g.GroupTeachers).ThenInclude(gt => gt.Teacher).Where(c => c.Id == 1).ToListAsync(); ;

            Group group = await _context.Groups.FirstOrDefaultAsync(g=>g.Id == 1);
            List<Group> groups1 = await _context.Groups.ToListAsync();
            List<Group> groups2 = await _context.Groups.Where(g=>g.StudentCount > 15).ToListAsync();

            if (await _context.Groups.AnyAsync(c=>c.Id == 1))
            {

            }

            return View(/*groups*/);
        }
    }
}
