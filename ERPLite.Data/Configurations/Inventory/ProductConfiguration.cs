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
            builder.HasIndex(p => p.SKU)
                    .IsUnique();
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

            builder.HasData(
new Product { Id = 1, Name = "Dell Latitude 5540", SKU = "PRD001", CostPrice = 25000, SellingPrice = 29000, QuantityInStock = 15, MinStockLevel = 5, CategoryId = 2, SupplierId = 1, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 2, Name = "HP ProBook 450", SKU = "PRD002", CostPrice = 22000, SellingPrice = 26000, QuantityInStock = 20, MinStockLevel = 5, CategoryId = 2, SupplierId = 2, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 3, Name = "Samsung 24 Monitor", SKU = "PRD003", CostPrice = 3500, SellingPrice = 4500, QuantityInStock = 25, MinStockLevel = 10, CategoryId = 9, SupplierId = 3, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 4, Name = "Logitech Mouse", SKU = "PRD004", CostPrice = 250, SellingPrice = 400, QuantityInStock = 100, MinStockLevel = 20, CategoryId = 3, SupplierId = 4, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 5, Name = "Logitech Keyboard", SKU = "PRD005", CostPrice = 450, SellingPrice = 650, QuantityInStock = 80, MinStockLevel = 20, CategoryId = 3, SupplierId = 4, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 6, Name = "TP-Link Router", SKU = "PRD006", CostPrice = 900, SellingPrice = 1300, QuantityInStock = 30, MinStockLevel = 10, CategoryId = 4, SupplierId = 5, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 7, Name = "Kingston SSD 1TB", SKU = "PRD007", CostPrice = 2200, SellingPrice = 2900, QuantityInStock = 40, MinStockLevel = 10, CategoryId = 5, SupplierId = 6, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 8, Name = "Canon Printer", SKU = "PRD008", CostPrice = 4500, SellingPrice = 5800, QuantityInStock = 12, MinStockLevel = 3, CategoryId = 6, SupplierId = 7, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 9, Name = "HDMI Cable", SKU = "PRD009", CostPrice = 70, SellingPrice = 150, QuantityInStock = 150, MinStockLevel = 30, CategoryId = 8, SupplierId = 8, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Product { Id = 10, Name = "CCTV Camera", SKU = "PRD010", CostPrice = 1800, SellingPrice = 2500, QuantityInStock = 18, MinStockLevel = 5, CategoryId = 10, SupplierId = 9, IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" }
);

        }
    }
}