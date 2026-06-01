using ERPLite.Services.DTOs.Inventory;

namespace ERPLite.Services.DTOs.Reports
{
    public class InventoryReportDto
    {
        public int TotalProducts { get; set; }

        public int LowStockProducts { get; set; }

        public int OutOfStockProducts { get; set; }

        public decimal InventoryValue { get; set; }

        public List<ProductDto> Products { get; set; } = new();
    }
}
