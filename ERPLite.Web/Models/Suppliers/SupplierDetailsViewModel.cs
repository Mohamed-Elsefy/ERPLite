using ERPLite.Services.DTOs.Inventory;

namespace ERPLite.Web.Models.Suppliers
{
    public class SupplierDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Address { get; set; }

        public IEnumerable<ProductDto> SuppliedProducts { get; set; } = new List<ProductDto>();

        public int TotalProductsCount => SuppliedProducts.Count();
        public decimal TotalFinancialVolume => SuppliedProducts.Sum(p => p.Price * p.QuantityInStock);
    }
}
