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
    }
}
