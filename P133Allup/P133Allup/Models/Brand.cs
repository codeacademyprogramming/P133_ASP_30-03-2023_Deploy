using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P133Allup.Models
{
    public class Brand : BaseEntity
    {
        [StringLength(255,ErrorMessage ="Qaqa Maksimum 255 Simvol")]
        [Required(ErrorMessage ="Mejburidi Valla")]
        //[Display(Name ="Brandin Adi")]
        public string Name { get; set; }
        [NotMapped]
        public string Desc { get; set; }

        public IEnumerable<Product>? Products { get; set; }
    }
}
