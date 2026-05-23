using ERPLite.Data.Entities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.System
{
    public class NotificationConfiguration
        : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(n => n.Message)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(n => n.Type)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(n => n.Priority)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}