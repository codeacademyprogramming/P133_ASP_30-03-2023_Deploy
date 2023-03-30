using System.ComponentModel.DataAnnotations.Schema;

namespace P133MentorHomeWork.Models
{
    public class PricingService
    {
        public int Id { get; set; }
        public int PricingId { get; set; }
        public Pricing Pricing { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
