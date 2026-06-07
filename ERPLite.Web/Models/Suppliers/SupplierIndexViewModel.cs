using ERPLite.Services.DTOs.Inventory;

namespace ERPLite.Web.Models.Suppliers
{
    public class SupplierIndexViewModel
    {
        public IEnumerable<SupplierDto> Suppliers { get; set; } = new List<SupplierDto>();
        public string? SearchTerm { get; set; }
    }
}
