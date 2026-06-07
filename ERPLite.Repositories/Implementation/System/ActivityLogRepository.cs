using ERPLite.Data.Context;
using ERPLite.Data.Entities.System;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.System;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.System
{
    public class ActivityLogRepository : GenericRepository<ActivityLog, int>, IActivityLogRepository
    {
        public ActivityLogRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ActivityLog>> GetRecentLogsAsync(int count)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.User)
                .OrderByDescending(x => x.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetByUserAsync(string userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<ActivityLog>> GetByEntityAsync(string entityName, int entityId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.EntityName == entityName && x.EntityId == entityId)
                .OrderByDescending(x => x.Timestamp)
                .ToListAsync();
        }
    }
}
