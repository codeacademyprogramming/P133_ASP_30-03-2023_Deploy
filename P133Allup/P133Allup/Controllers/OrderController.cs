using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P133Allup.DataAccessLayer;
using P133Allup.Hubs;
using P133Allup.Models;
using P133Allup.ViewModels.BasketViewModels;
using P133Allup.ViewModels.OrderViewsModels;

namespace P133Allup.Controllers
{
    [Authorize(Roles = "Member")]
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public OrderController(UserManager<AppUser> userManager, AppDbContext context, IHubContext<NotificationHub> hub)
        {
            _userManager = userManager;
            _context = context;
            _hub = hub;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            string coockie = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(coockie))
            {
                return RedirectToAction("Index", "Product");
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);

                basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                basketVM.Title = product.Title;
            }

            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsMain && a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            Order order = new Order
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                Email = appUser.Email,
                Phone = appUser.PhoneNumber,
                AddressLine = appUser.Addresses?.FirstOrDefault().AddressLine,
                City = appUser.Addresses?.FirstOrDefault().City,
                Country = appUser.Addresses?.FirstOrDefault().Country,
                State = appUser.Addresses?.FirstOrDefault().State,
                PostalCode = appUser.Addresses?.FirstOrDefault().PostalCode
            };

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
                .Include(u=>u.Orders)
                .Include(u => u.Addresses.Where(a => a.IsMain && a.IsDeleted == false))
                .Include(u=>u.Baskets.Where(b=>b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            string coockie = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(coockie))
            {
                return RedirectToAction("Index", "Product");
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);

                basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                basketVM.Title = product.Title;
            }

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs
            };

            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (BasketVM basketVM in basketVMs)
            {
                OrderItem orderItem = new OrderItem
                {
                    Count = basketVM.Count,
                    ProductId = basketVM.Id,
                    Price = basketVM.Price,
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}"
                };

                orderItems.Add(orderItem);
            }

            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;
            }

            HttpContext.Response.Cookies.Append("basket", "");

            order.UserId = appUser.Id;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.OrderItems = orderItems;
            order.No = appUser.Orders != null && appUser.Orders.Count() > 0 ? appUser.Orders.Last().No + 1 : 1;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            if (true)
            {

            }

            return RedirectToAction("index","home");
        }
    }
}
