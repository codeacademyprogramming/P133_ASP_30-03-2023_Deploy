using P133Allup.Models;
using P133Allup.ViewModels.BasketViewModels;

namespace P133Allup.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<BasketVM>> GetBaskets();
    }
}
