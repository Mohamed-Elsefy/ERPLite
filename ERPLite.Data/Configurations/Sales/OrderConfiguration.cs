using ERPLite.Data.Entities.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.Sales
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(o => o.OrderDate)
                .IsRequired();

            builder.HasIndex(o => o.OrderDate);

            builder.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(o => o.CreatedByUser)
                .WithMany()
                .HasForeignKey(o => o.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}