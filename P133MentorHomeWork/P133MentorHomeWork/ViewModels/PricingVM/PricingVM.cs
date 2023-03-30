using P133MentorHomeWork.Models;

namespace P133MentorHomeWork.ViewModels.PricingVM
{
    public class PricingVM
    {
        public IEnumerable<Pricing> Pricings { get; set; }
        public IEnumerable<Service> Services { get; set; }
    }
}
