namespace ERPLite.Services.DTOs.Dashboard
{
    public class SalesAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal TotalPaidRevenue { get; set; }

        public decimal OutstandingBalance { get; set; }

        public int TotalOrders { get; set; }

        public int PaidOrders { get; set; }

        public int UnpaidOrders { get; set; }
    }
}
