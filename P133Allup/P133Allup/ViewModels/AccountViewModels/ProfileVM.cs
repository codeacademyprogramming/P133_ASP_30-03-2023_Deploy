using P133Allup.Models;

namespace P133Allup.ViewModels.AccountViewModels
{
    public class ProfileVM
    {
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
