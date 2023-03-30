using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P133Allup.DataAccessLayer;

namespace P133Allup.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(IDictionary<string, string> settings)
        {
            //IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(a=>a.Key,a=>a.Value);

            return View(await Task.FromResult(settings));
        }
    }
}
