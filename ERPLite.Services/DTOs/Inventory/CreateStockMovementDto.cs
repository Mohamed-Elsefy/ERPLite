using ERPLite.Data.Entities.Inventory;

namespace ERPLite.Services.DTOs.Inventory
{
    public class CreateStockMovementDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public StockMovementType Type { get; set; }

        public string? Notes { get; set; }
    }
}
