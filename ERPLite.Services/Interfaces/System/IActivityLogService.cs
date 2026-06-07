using ERPLite.Services.DTOs.System;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.System
{
    public interface IActivityLogService
    {
        Task LogAsync(string userId, string action, string entityName, int entityId, string description);

        Task<ServiceResult<IEnumerable<ActivityLogDto>>> GetRecentLogsAsync();
        Task<ServiceResult<IEnumerable<ActivityLogDto>>> GetUserLogsAsync(string userId);
        Task<ServiceResult<IEnumerable<ActivityLogDto>>> GetEntityLogsAsync(string entityName, int entityId);

        Task<ServiceResult<ActivityLogDto>> GetLogByIdAsync(int id);
    }
}
