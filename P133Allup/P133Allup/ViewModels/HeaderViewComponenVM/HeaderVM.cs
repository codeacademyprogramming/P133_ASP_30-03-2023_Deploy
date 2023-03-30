using P133Allup.Models;
using P133Allup.ViewModels.BasketViewModels;

namespace P133Allup.ViewModels.HeaderViewComponenVM
{
    public class HeaderVM
    {
        public IDictionary<string,string> Settings { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<BasketVM> BasketVMs { get; set; }
    }
}
