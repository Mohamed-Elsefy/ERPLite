using ERPLite.Data.Constants;

namespace ERPLite.Data.Entities.Inventory
{
    public class Supplier : BaseEntity, ISoftDeletable
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Phone { get; set; } = null!;
        public string? Email { get; set; }

        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}