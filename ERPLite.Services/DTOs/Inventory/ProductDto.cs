namespace ERPLite.Services.DTOs.Inventory
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = null!;
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int MinStockLevel { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }
}
