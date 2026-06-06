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

            builder.HasData(
new Supplier { Id = 1, Name = "TechSource", Phone = "01010000001", Email = "[info@techsource.com](mailto:info@techsource.com)", Address = "Cairo", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 2, Name = "Delta IT", Phone = "01010000002", Email = "[sales@deltait.com](mailto:sales@deltait.com)", Address = "Alexandria", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 3, Name = "Future Electronics", Phone = "01010000003", Email = "[contact@future.com](mailto:contact@future.com)", Address = "Giza", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 4, Name = "Smart Solutions", Phone = "01010000004", Email = "[info@smart.com](mailto:info@smart.com)", Address = "Mansoura", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 5, Name = "Mega Store", Phone = "01010000005", Email = "[sales@mega.com](mailto:sales@mega.com)", Address = "Tanta", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 6, Name = "Digital World", Phone = "01010000006", Email = "[info@digital.com](mailto:info@digital.com)", Address = "Cairo", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 7, Name = "Prime Tech", Phone = "01010000007", Email = "[support@primetech.com](mailto:support@primetech.com)", Address = "Alexandria", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 8, Name = "Elite Supplies", Phone = "01010000008", Email = "[info@elite.com](mailto:info@elite.com)", Address = "Ismailia", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 9, Name = "Pro Devices", Phone = "01010000009", Email = "[sales@prodevices.com](mailto:sales@prodevices.com)", Address = "Port Said", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" },
new Supplier { Id = 10, Name = "Global IT", Phone = "01010000010", Email = "[contact@globalit.com](mailto:contact@globalit.com)", Address = "Cairo", IsActive = true, CreatedAtUtc = new DateTimeOffset(new DateTime(2025, 1, 1)), CreatedBy = "System" }
);

        }
    }
}