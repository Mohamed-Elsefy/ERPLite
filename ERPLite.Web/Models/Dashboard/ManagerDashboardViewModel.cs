using ERPLite.Services.DTOs.Dashboard;

namespace ERPLite.Web.Models.Dashboard
{
    public class ManagerDashboardViewModel
    {
        public DashboardStatisticsDto Statistics { get; set; } = new();
    }
}
