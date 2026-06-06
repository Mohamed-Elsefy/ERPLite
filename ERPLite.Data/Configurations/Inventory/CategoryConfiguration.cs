using ERPLite.Data.Entities.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERPLite.Data.Configurations.Inventory
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasData(
new Category { Id = 1, Name = "Electronics", Description = "Electronic Devices", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 2, Name = "Computers", Description = "Computers & Laptops", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 3, Name = "Accessories", Description = "Accessories", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 4, Name = "Networking", Description = "Networking Devices", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 5, Name = "Storage", Description = "Storage Devices", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 6, Name = "Printers", Description = "Printers & Scanners", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 7, Name = "Office Supplies", Description = "Office Equipment", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 8, Name = "Cables", Description = "Cables & Connectors", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 9, Name = "Monitors", Description = "Displays & Monitors", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Category { Id = 10, Name = "Security", Description = "Security Systems", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" }
);

        }
    }
}