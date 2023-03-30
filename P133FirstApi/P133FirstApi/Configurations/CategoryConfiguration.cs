using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P133FirstApi.Entities;

namespace P133FirstApi.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            //builder.HasKey(x => x.Id);

            //builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(b => b.CreatedBy).HasDefaultValue("System");
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(b=>b.Name).HasMaxLength(100).IsRequired(false);
            //One To Many
            //builder.HasOne(x=>x.Parent).WithMany(x=>x.Children).HasForeignKey(x=>x.ParentId).OnDelete(DeleteBehavior.Restrict);

            //One To One
            //builder.HasOne(x => x.TestOne).WithOne(x => x.Category).HasForeignKey<Category>(x => x.TestOneId);
        }
    }
}
