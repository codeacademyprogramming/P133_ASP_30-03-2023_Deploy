using Microsoft.EntityFrameworkCore;
using P133MentorHomeWork.Models;

namespace P133MentorHomeWork.DataAccessLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Pricing> Pricings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<PricingService> PricingServices { get; set; }
        public DbSet<Feature> Features { get; set; }
    }
}
