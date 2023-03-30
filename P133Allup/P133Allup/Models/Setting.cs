using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P133Allup.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string? Key { get; set; }
        [StringLength(1000)]
        public string? Value { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
