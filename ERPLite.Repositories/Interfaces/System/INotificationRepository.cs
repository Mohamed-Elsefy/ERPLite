using ERPLite.Data.Entities.System;

namespace ERPLite.Repositories.Interfaces.System
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId);
        Task<int> GetUnreadCountByUserIdAsync(string userId);
        Task AddAsync(Notification notification);
        Task<Notification?> GetByIdAsync(int id);
        void Update(Notification notification);
    }
}
