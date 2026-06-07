using ERPLite.Services.DTOs.Sales;

namespace ERPLite.Web.Models.Customers
{
    public class CustomerDetailsViewModel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime OnboardedAt { get; set; }
        public List<OrderDto> OrdersPipeline { get; set; } = new();
        public decimal TotalLifetimeValue => OrdersPipeline?.Sum(o => o.TotalPrice) ?? 0;
    }
}
