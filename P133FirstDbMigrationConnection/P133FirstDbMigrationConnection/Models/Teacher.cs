namespace P133FirstDbMigrationConnection.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }

        public ICollection<GroupTeacher> GroupTeachers { get; set; }
    }
}
