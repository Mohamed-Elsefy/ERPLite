using ERPLite.Services.DTOs.Sales;
using ERPLite.Web.Models.Common;

namespace ERPLite.Web.Models.Orders
{
    public class OrderIndexViewModel
    {
        public IEnumerable<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
