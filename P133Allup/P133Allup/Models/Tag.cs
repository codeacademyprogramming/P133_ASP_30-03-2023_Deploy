using System.ComponentModel.DataAnnotations;

namespace P133Allup.Models
{
    public class Tag : BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
    }
}
