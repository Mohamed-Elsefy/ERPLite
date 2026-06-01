using ERPLite.Data.Entities.System;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.System
{
    public interface IActivityLogRepository : IGenericRepository<ActivityLog, int>
    {
        Task<IEnumerable<ActivityLog>> GetRecentLogsAsync(int count);

        Task<IEnumerable<ActivityLog>> GetByUserAsync(string userId);

        Task<IEnumerable<ActivityLog>> GetByEntityAsync(string entityName, int entityId);
    }
}
