using ERPLite.Services.DTOs.Sales;
using ERPLite.Web.Models.Common;

namespace ERPLite.Web.Models.Customers
{
    public class CustomerIndexViewModel
    {
        public IEnumerable<CustomerDto> Customers { get; set; } = new List<CustomerDto>();
        public string? SearchTerm { get; set; }
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
