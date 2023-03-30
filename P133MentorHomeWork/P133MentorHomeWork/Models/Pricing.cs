using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P133MentorHomeWork.Models
{
    public class Pricing
    {
        public int Id { get; set; }
        [StringLength(255),Required]
        public string Name { get; set; }
        [Column(TypeName ="money")]
        public double Price { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsAdvanced { get; set; }

        public IEnumerable<PricingService> PricingServices { get; set; }
    }
}
