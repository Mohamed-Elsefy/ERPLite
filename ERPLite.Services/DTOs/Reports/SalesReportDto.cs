using ERPLite.Services.DTOs.Sales;

namespace ERPLite.Services.DTOs.Reports
{
    public class SalesReportDto
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalSales { get; set; }

        public decimal AverageOrderValue { get; set; }

        public List<OrderDto> Orders { get; set; } = new();
    }
}
