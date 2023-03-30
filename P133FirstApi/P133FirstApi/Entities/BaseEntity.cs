namespace P133FirstApi.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string? CreatedBy { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
