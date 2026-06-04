using ERPLite.Data.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.Inventory
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.CostPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.SellingPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.QuantityInStock)
                .IsRequired();

            builder.Property(p => p.MinStockLevel)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(p => p.Name);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}