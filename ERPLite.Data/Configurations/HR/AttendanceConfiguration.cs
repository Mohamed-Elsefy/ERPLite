using ERPLite.Data.Entities.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.HR
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(a => new { a.EmployeeId, a.Date })
                .HasFilter("[EmployeeId] IS NOT NULL")
                .IsUnique();
        }
    }
}