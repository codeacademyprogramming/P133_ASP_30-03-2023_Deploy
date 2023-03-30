using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P133Allup.DataAccessLayer;
using P133Allup.Interfaces;
using P133Allup.Models;
using P133Allup.ViewModels.BasketViewModels;

namespace P133Allup.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BasketVM>> GetBaskets()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _appDbContext.Products
                        .FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        basketVM.ExTax = product.ExTax;
                        basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        basketVM.Title= product.Title;
                        basketVM.Image = product.MainImage;
                    }
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            return basketVMs;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _appDbContext.Categories.Include(c=>c.Children).Where(c=>c.IsMain && c.IsDeleted == false).ToListAsync();
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string,string> settings = await _appDbContext.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);

            return settings;
        }
    }
}
