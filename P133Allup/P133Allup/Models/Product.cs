using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P133Allup.Models
{
    public class Product : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [Column(TypeName ="money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]
        public double DiscountedPrice { get; set; }
        [Column(TypeName = "money")]
        public double ExTax { get; set; }
        public int Count { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(4,MinimumLength =4)]
        public string? Seria { get; set; }
        public int Code { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsFeatured { get; set; }
        [StringLength(255)]
        public string? MainImage { get; set; }
        [StringLength(255)]
        public string? HoverImage { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public IEnumerable<ProductTag>? ProductTags { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public IEnumerable<Basket>? Baskets { get; set; }

        [NotMapped]
        public IEnumerable<byte>? TagIds { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        [NotMapped]
        public IFormFile? MainFile { get; set; }
        [NotMapped]
        public IFormFile? HoverFile { get; set; }

    }
}
