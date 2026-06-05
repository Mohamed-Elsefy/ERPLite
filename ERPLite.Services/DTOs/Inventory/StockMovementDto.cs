using ERPLite.Data.Entities.Inventory;

namespace ERPLite.Services.DTOs.Inventory
{
    public class StockMovementDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }

        public StockMovementType Type { get; set; }

        public string? Notes { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
