using ERPLite.Services.DTOs.Sales;

namespace ERPLite.Web.Models.Customers
{
    public class CustomerIndexViewModel
    {
        public IEnumerable<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        public string? SearchTerm { get; set; }
    }
}
