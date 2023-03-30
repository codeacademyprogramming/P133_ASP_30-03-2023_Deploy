using System.ComponentModel.DataAnnotations;

namespace P133MentorHomeWork.Models
{
    public class Service
    {
        public int Id { get; set; }
        [StringLength(255),Required]
        public string Name { get; set; }

        public IEnumerable<PricingService> PricingServices { get; set; }
    }
}
