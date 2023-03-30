using Microsoft.EntityFrameworkCore;
using P133FirstDbMigrationConnection.Models;

namespace P133FirstDbMigrationConnection.DataAccessLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<GroupTeacher> GroupTeachers { get; set; }
    }
}
