namespace ERPLite.Services.DTOs.Inventory
{
    public class UpdateProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

        public int MinStockLevel { get; set; }

        public int CategoryId { get; set; }

        public int SupplierId { get; set; }
    }
}
