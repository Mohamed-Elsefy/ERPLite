using ERPLite.Services.DTOs.Inventory;

namespace ERPLite.Web.Models.Products
{
    public class ProductIndexViewModel
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public string? SearchTerm { get; set; }
    }
}
