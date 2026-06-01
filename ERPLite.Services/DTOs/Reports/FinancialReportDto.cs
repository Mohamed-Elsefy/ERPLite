namespace ERPLite.Services.DTOs.Reports
{
    public class FinancialReportDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal OutstandingBalance { get; set; }

        public int PaidOrders { get; set; }

        public int UnpaidOrders { get; set; }
    }
}
