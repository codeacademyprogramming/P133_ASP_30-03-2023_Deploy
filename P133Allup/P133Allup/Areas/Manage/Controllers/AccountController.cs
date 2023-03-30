using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.Areas.Manage.ViewModels.AccountVMs;
using P133Allup.Models;

namespace P133Allup.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)  return View(registerVM);

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                FatherName = registerVM.FatherName,
                Email = registerVM.Email,
                UserName = registerVM.UserName
            };

            //if (await _userManager.Users.AnyAsync(s=>s.NormalizedEmail == registerVM.Email.ToUpperInvariant().Trim()))
            //{
            //    ModelState.AddModelError("Email", $"{registerVM.Email} Artiq Isdifade Olunub");
            //    return View(registerVM);
            //}

            //if (await _userManager.Users.AnyAsync(s => s.NormalizedUserName == registerVM.UserName.ToUpperInvariant().Trim()))
            //{
            //    ModelState.AddModelError("UserName", $"{registerVM.Email} Artiq Isdifade Olunub");
            //    return View(registerVM);
            //}

            IdentityResult identityResult = await _userManager.CreateAsync(appUser,registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Admin");

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);

            if (appUser == null) 
            {
                ModelState.AddModelError("", "Email ve ya Sifre Yanlisdir");
                return View(loginVM);
            }

            //if (!await _userManager.CheckPasswordAsync(appUser,loginVM.Password))
            //{
            //    ModelState.AddModelError("Password", "Password Yanlisdir");
            //    return View(loginVM);
            //}

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);

            if (!signInResult.Succeeded) 
            {
                ModelState.AddModelError("", "Email ve ya Sifre Yanlisdir");
                return View(loginVM);
            }

            return RedirectToAction("index", "dashboard", new { areas = "manage" });
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles ="Admin,SuperAdmin")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            ProfileVM profileVM = new ProfileVM
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                FatherName = appUser.FatherName,
                Email = appUser.Email,
                UserName = appUser.UserName,
            };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            if (!ModelState.IsValid)
            {
                return View(profileVM);
            }

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.Name = profileVM.Name;
            appUser.SurName = profileVM.SurName;
            appUser.FatherName = profileVM.FatherName;

            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                appUser.Email = profileVM.Email;
            }

            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.UserName;
            }

            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }

                return View(profileVM);
            }

            await _signInManager.SignInAsync(appUser, true);

            if (!string.IsNullOrWhiteSpace(profileVM.OldPassword))
            {
                if (!await _userManager.CheckPasswordAsync(appUser,profileVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old Password Yanlisdir");
                    return View(profileVM);
                }

                if (profileVM.OldPassword == profileVM.Password)
                {
                    ModelState.AddModelError("Password", "Sifre Kohne Sifreynen Eyni Ola Bilmez");
                    return View(profileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);

                identityResult = await _userManager.ResetPasswordAsync(appUser, token, profileVM.Password);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }

                    return View(profileVM);
                }
            }

            return RedirectToAction("index", "dashboard", new { areas = "manage" });
        }

        #region Create Role And Super Admin
        //[HttpGet]
        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));

        //    return Content("Ugurlu Oldu");
        //}

        //[HttpGet]
        //public async Task<IActionResult> CreateUser()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        Name = "Super",
        //        SurName = "Admin",
        //        FatherName = "SuperAdminFather",
        //        UserName = "SuperAdmin",
        //        Email = "superadmin@gmail.com"
        //    };

        //    await _userManager.CreateAsync(appUser, "SuperAdmin133");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Content("Ugurlu Oldu");
        //}
        #endregion
    }
}
