using ERPLite.Services.DTOs.Inventory;

namespace ERPLite.Web.Models.Categories
{
    public class CategoryIndexViewModel
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public string? SearchTerm { get; set; }
    }
}
