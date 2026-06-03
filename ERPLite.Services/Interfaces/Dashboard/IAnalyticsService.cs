using ERPLite.Services.DTOs.Dashboard;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Dashboard
{
    public interface IAnalyticsService
    {
        Task<ServiceResult<SalesAnalyticsDto>> GetSalesAnalyticsAsync();
    }
}
