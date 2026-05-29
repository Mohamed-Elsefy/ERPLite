using ERPLite.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.System
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.ToTable("ActivityLogs");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.Action)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(al => al.EntityName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(al => al.Timestamp)
                .IsRequired();

            builder.HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}