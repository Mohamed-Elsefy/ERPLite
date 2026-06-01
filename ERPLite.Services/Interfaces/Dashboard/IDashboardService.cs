using ERPLite.Services.DTOs.Dashboard;

namespace ERPLite.Services.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardStatisticsDto> GetStatisticsAsync();
    }
}
