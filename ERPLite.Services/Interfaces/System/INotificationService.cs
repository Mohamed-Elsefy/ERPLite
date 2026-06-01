using ERPLite.Services.DTOs.System;

namespace ERPLite.Services.Interfaces.System
{
    public interface INotificationService
    {
        Task SendAsync(NotificationDto notification);
    }
}
