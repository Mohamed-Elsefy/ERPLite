namespace ERPLite.Services.DTOs.Dashboard
{
    public class DashboardStatisticsDto
    {
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int LowStockProducts { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int NewCustomersThisMonth { get; set; }
        public int NewOrdersThisMonth { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int ActiveCustomers { get; set; }
        public int PaidOrders { get; set; }
        public int UnpaidOrders { get; set; }
        public decimal OutstandingBalance { get; set; }
        public decimal TotalPaidRevenue { get; set; }
        public int PartialPaidOrders { get; set; }
    }
}
