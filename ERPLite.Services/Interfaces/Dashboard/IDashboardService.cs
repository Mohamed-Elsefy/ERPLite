using ERPLite.Services.DTOs.Dashboard;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<ServiceResult<DashboardStatisticsDto>> GetStatisticsAsync();
    }
}
