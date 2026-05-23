using ERPLite.Data.Entities.Sales;

namespace ERPLite.Data.Entities.Inventory
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

        public int MinStockLevel { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public int SupplierId { get; set; }

        public Supplier Supplier { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}