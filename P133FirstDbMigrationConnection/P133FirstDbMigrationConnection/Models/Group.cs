namespace P133FirstDbMigrationConnection.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public byte StudentCount { get; set; }

        public ICollection<Student> Students { get; set; }
        public ICollection<GroupTeacher> GroupTeachers { get; set; }

    }
}
