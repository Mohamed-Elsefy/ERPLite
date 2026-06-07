using ERPLite.Services.DTOs.System;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.System
{
    public interface INotificationService
    {
        Task SendAsync(NotificationDto notification);
        Task<ServiceResult<int>> CreateSystemNotificationAsync(string userId, string title, string message, string type, string priority);
        Task<ServiceResult<IEnumerable<SystemNotificationDto>>> GetUserNotificationsAsync(string userId);
        Task<ServiceResult<int>> GetUnreadCountAsync(string userId);
        Task<ServiceResult> MarkAsReadAsync(int notificationId);
        Task<ServiceResult<SystemNotificationDto>> GetByIdAsync(int id);
    }
}
