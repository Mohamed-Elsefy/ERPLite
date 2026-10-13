using ERPLite.Services.DTOs.Inventory;
using ERPLite.Web.Models.Common;

namespace ERPLite.Web.Models.Products
{
    public class ProductIndexViewModel
    {
        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public string? SearchTerm { get; set; }
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
