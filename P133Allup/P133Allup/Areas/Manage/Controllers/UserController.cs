using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.Areas.Manage.ViewModels.UserVMs;
using P133Allup.DataAccessLayer;
using P133Allup.Models;
using P133Allup.ViewModels;

namespace P133Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager,
            AppDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context= context;
            _roleManager= roleManager;
        }

        public async Task<IActionResult> Index(int pageIndex =1)
        {
            List<UserVM> query = await _userManager.Users
                .Where(u=>u.UserName != User.Identity.Name)
                .Select(x => new UserVM
                {
                    Id = x.Id,
                    Email= x.Email,
                    Name =x.Name,
                    SurName = x.SurName,
                    UserName= x.UserName
                })
                .ToListAsync();

            
            foreach (var item in query)
            {
                string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == item.Id).RoleId;
                string roleName = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;

                item.RoleName = roleName;
            }

            return View(PageNatedList<UserVM>.Create(query.AsQueryable(),pageIndex,3));
        }

        [HttpGet]
        public async Task<IActionResult> ChangeRole(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            AppUser user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;

            UserChangeRoleVM userChangeRoleVM = new UserChangeRoleVM
            {
                UserId = user.Id,
                RoleId = roleId
            };

            ViewBag.Role = await _roleManager.Roles.Where(c => c.Name != "SuperAdmin").ToListAsync();

            return View(userChangeRoleVM);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(UserChangeRoleVM userChangeRoleVM)
        {
            ViewBag.Role = await _roleManager.Roles.Where(c => c.Name != "SuperAdmin").ToListAsync();

            if (!ModelState.IsValid) return View(userChangeRoleVM);

            if (string.IsNullOrWhiteSpace(userChangeRoleVM.UserId)) return BadRequest();

            AppUser user = await _userManager.FindByIdAsync(userChangeRoleVM.UserId);

            if (user == null) return NotFound();

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == userChangeRoleVM.UserId).RoleId;
            string roleName = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;

            string newRoleName = _roleManager.Roles
                .FirstOrDefault(c => c.Name != "SuperAdmin" && c.Id == userChangeRoleVM.RoleId).Name;
            await _userManager.RemoveFromRoleAsync(user, roleName);
            await _userManager.AddToRoleAsync(user, newRoleName);
            return RedirectToAction(nameof(Index));
        }
    }
}
