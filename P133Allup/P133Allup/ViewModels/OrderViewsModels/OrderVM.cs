using P133Allup.Models;
using P133Allup.ViewModels.BasketViewModels;

namespace P133Allup.ViewModels.OrderViewsModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IEnumerable<BasketVM> BasketVMs { get; set; }
    }
}
