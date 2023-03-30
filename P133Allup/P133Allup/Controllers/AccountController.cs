using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using P133Allup.DataAccessLayer;
using P133Allup.Models;
using P133Allup.ViewModels.AccountViewModels;
using P133Allup.ViewModels.BasketViewModels;
using MailKit.Net.Smtp;
using P133Allup.ViewModels;
using Microsoft.Extensions.Options;

namespace P133Allup.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly SmtpSetting _smtpSetting;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppDbContext context,
            IConfiguration configuration,
            IOptions<SmtpSetting> smtpSetting)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _smtpSetting = smtpSetting.Value;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                FatherName = registerVM.FatherName,
                Email = registerVM.Email,
                UserName = registerVM.UserName
            };

            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }

            await _userManager.AddToRoleAsync(appUser, "Member");
            string token =  await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

            string url = Url.Action("EmailConfirm", "Account", new {id=appUser.Id, token=token},HttpContext.Request.Scheme, HttpContext.Request.Host.ToString());

            string templateFullPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Shared", "_EmailConfirmPartial.cshtml");
            string templateContent = await System.IO.File.ReadAllTextAsync(templateFullPath);
            //templateContent = templateContent.Replace("{{name}}", appUser.Name);
            //templateContent = templateContent.Replace("{{surname}}", appUser.SurName);
            templateContent = templateContent.Replace("{{url}}", url);

            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_smtpSetting.Email));
            //mimeMessage.From.Add(MailboxAddress.Parse(_configuration.GetSection("SmtpSetting:Email").Value));
            //mimeMessage.From.Add(MailboxAddress.Parse("p133codeacademy@gmail.com"));
            mimeMessage.To.Add(MailboxAddress.Parse(appUser.Email));
            mimeMessage.Subject = "Email Confirmation";
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text =templateContent
            };
            //mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            //{
            //    Text = $"<a href='{url}'>Confirm Email</a>"
            //};
            //mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            //{
            //    Text = "Email Confirm"
            //};

            using (SmtpClient smtpClient = new SmtpClient())
            {
                await smtpClient.ConnectAsync(_smtpSetting.Host, _smtpSetting.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_smtpSetting.Email, _smtpSetting.Password);
                await smtpClient.SendAsync(mimeMessage);
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }

            //using (SmtpClient smtpClient = new SmtpClient())
            //{
            //    await smtpClient.ConnectAsync(_configuration.GetSection("SmtpSetting:Host").Value, int.Parse(_configuration.GetSection("SmtpSetting:Port").Value), MailKit.Security.SecureSocketOptions.StartTls);
            //    await smtpClient.AuthenticateAsync(_configuration.GetSection("SmtpSetting:Email").Value, _configuration.GetSection("SmtpSetting:Password").Value);
            //    await smtpClient.SendAsync(mimeMessage);
            //    await smtpClient.DisconnectAsync(true);
            //    smtpClient.Dispose();
            //}

            //using (SmtpClient smtpClient = new SmtpClient())
            //{
            //    await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //    await smtpClient.AuthenticateAsync("p133codeacademy@gmail.com", "gjfdotdkhfyldisq");
            //    await smtpClient.SendAsync(mimeMessage);
            //    await smtpClient.DisconnectAsync(true);
            //    smtpClient.Dispose();
            //}

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
            if (!ModelState.IsValid) return View(loginVM);

            AppUser appUser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View(loginVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RemindMe, true);


            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Hesabiniz Bloklanib");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email Or Password Is InCorrect");
                return View(loginVM);
            }

            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
                {
                    List<BasketVM> basketVMs = new List<BasketVM>();

                    foreach (Basket basket1 in appUser.Baskets)
                    {
                        BasketVM basketVM = new BasketVM
                        {
                            Id = (int)basket1.ProductId,
                            Count = basket1.Count,
                        };

                        basketVMs.Add(basketVM);
                    }

                    basket = JsonConvert.SerializeObject(basketVMs);

                    HttpContext.Response.Cookies.Append("basket", basket);
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append("basket", "");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders.Where(o => o.IsDeleted == false))
                .ThenInclude(o => o.OrderItems.Where(oi => oi.IsDeleted == false))
                .ThenInclude(oi => oi.Product)
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses,
                Orders = appUser.Orders
            };

            return View(profileVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses
            };

            if (!ModelState.IsValid)
            {
                return View(nameof(Profile), profileVM);
            }

            if (address.IsMain && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(u => u.IsMain))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain).IsMain = false;
            }

            address.UserId = appUser.Id;
            address.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            address.CreatedAt = DateTime.UtcNow.AddHours(4);


            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            TempData["Tab"] = "address";

            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public async Task<IActionResult> EmailConfirm(string id,string token)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(appUser, token);

            if (!identityResult.Succeeded)
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(appUser, false);

            return RedirectToAction("index","home");
        }
    }
}