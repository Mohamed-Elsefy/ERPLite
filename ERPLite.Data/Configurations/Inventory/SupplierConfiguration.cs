using ERPLite.Data.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.Inventory
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(s => s.Phone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.Address)
                .HasMaxLength(250);

            builder.Property(s => s.IsActive)
                .HasDefaultValue(true);

            builder.HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}