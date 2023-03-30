using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;
using P133Allup.Extentions;
using P133Allup.Helpers;
using P133Allup.Models;
using P133Allup.ViewModels;
using System.Data;

namespace P133Allup.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IEnumerable<Setting> settings = await _context.Settings.ToListAsync();

            //ViewBag.TotalPage = (int)Math.Ceiling((decimal)brands.Count() / 3);
            //ViewBag.PageIndex = pageIndex;

            //brands = brands.Skip((pageIndex - 1) * 3).Take(3);

            return View(settings);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Setting setting = await _context.Settings.FirstOrDefaultAsync(s=>s.Id == id);

            if (setting == null) return NotFound();

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Setting setting)
        {
            if (!ModelState.IsValid) return View(setting);

            if (id == null) return BadRequest();

            if(id != setting.Id) return BadRequest();

            Setting dbSetting = await _context.Settings.FirstOrDefaultAsync(s=>s.Id ==id);

            if (dbSetting == null) return NotFound();

            if (id == 2)
            {
                if (setting.File != null)
                {
                    if (!setting.File.CheckFileContentType("image/png"))
                    {
                        ModelState.AddModelError("File", "Fayl Tipi Duz Deyil");
                        return View(setting);
                    }

                    if (!setting.File.CheckFileLength(300))
                    {
                        ModelState.AddModelError("File", "Fayl Olcusu Maksimum 30 kb Ola Biler");
                        return View(setting);
                    }

                    FileHelper.DeleteFile(dbSetting.Value, _env, "assets", "images");

                    dbSetting.Value = await setting.File.CraeteFileAsync(_env, "assets", "images");
                }
            }
            else
            {
                dbSetting.Value = setting.Value;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
