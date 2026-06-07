using ERPLite.Data.Context;
using ERPLite.Data.Entities.System;
using ERPLite.Repositories.Interfaces.System;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.System
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
        }
    }
}
