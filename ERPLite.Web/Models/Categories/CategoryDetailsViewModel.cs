using ERPLite.Services.DTOs.Inventory;

namespace ERPLite.Web.Models.Categories
{
    public class CategoryDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IEnumerable<ProductDto> AssociatedProducts { get; set; } = new List<ProductDto>();
        public int TotalProductsCount => AssociatedProducts.Count();
        public decimal TotalValue => AssociatedProducts.Sum(p => p.Price * p.QuantityInStock);
    }
}
