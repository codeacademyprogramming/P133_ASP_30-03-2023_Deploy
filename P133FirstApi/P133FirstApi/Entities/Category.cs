using System.ComponentModel.DataAnnotations.Schema;

namespace P133FirstApi.Entities
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        //[NotMapped]
        //public IFormFile File { get; set; }

        public int? ParentId { get; set; }
        public Category? Parent { get; set; }

        public IEnumerable<Category>? Children { get; set; }
        public IEnumerable<Product> Products { get; set; }

        //public int? TestOneId { get; set; }
        //public TestOne? TestOne { get; set; }
    }
}
