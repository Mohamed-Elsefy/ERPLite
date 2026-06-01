using ERPLite.Services.DTOs.Dashboard;

namespace ERPLite.Web.Areas.Admin.Models.Dashboard
{
    public class AdminDashboardViewModel
    {

        public DashboardStatisticsDto Statistics { get; set; } = null!;

        public SalesAnalyticsDto Analytics { get; set; } = null!;
    }
}
