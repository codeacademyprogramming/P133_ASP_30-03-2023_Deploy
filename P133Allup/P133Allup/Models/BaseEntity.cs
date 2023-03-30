using System.ComponentModel.DataAnnotations;

namespace P133Allup.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        [StringLength(255)]
        public string? UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }
        [StringLength(255)]
        public string? DeletedBy { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }
    }
}
