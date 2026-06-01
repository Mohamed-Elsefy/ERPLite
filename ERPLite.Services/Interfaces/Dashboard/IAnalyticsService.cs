using ERPLite.Services.DTOs.Dashboard;

namespace ERPLite.Services.Interfaces.Dashboard
{
    public interface IAnalyticsService
    {
        Task<SalesAnalyticsDto> GetSalesAnalyticsAsync();
    }
}
