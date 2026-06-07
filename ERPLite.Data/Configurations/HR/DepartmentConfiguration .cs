using ERPLite.Data.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.HR
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.Description)
                .HasMaxLength(500);

            builder.HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}