using AutoMapper;
using ERPLite.Data.Entities.System;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.System;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.System;
using ERPLite.Services.Interfaces.Infrastructure;
using Microsoft.Extensions.Logging;

namespace ERPLite.Services.Services.System
{
    public class DbNotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DbNotificationService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DbNotificationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DbNotificationService> logger,
            IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task SendAsync(NotificationDto notification)
        {
            _logger.LogInformation("Email Sent To {Recipient} | Title: {Title} | Message: {Message}",
                notification.Recipient, notification.Title, notification.Message);

            await Task.CompletedTask;
        }

        public async Task<ServiceResult<int>> CreateSystemNotificationAsync(string userId, string title, string message, string type, string priority)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    Priority = priority,
                    IsRead = false,
                    CreatedAt = _dateTimeProvider.UtcNow
                };

                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<int>.Successful(notification.Id, "Internal notification dispatch successful.");
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Failed($"Failed to log infrastructure notification: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<SystemNotificationDto>>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _unitOfWork.Notifications.GetNotificationsByUserIdAsync(userId);
            var dto = _mapper.Map<IEnumerable<SystemNotificationDto>>(notifications);
            return ServiceResult<IEnumerable<SystemNotificationDto>>.Successful(dto);
        }

        public async Task<ServiceResult<int>> GetUnreadCountAsync(string userId)
        {
            var count = await _unitOfWork.Notifications.GetUnreadCountByUserIdAsync(userId);
            return ServiceResult<int>.Successful(count);
        }

        public async Task<ServiceResult> MarkAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null) return ServiceResult.Failed("Notification instance not found.");

            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult.Successful("Notification vector state flagged as read.");
        }

        public async Task<ServiceResult<SystemNotificationDto>> GetByIdAsync(int id)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(id);
            if (notification == null) return ServiceResult<SystemNotificationDto>.Failed("Notification record absent.");

            var dto = _mapper.Map<SystemNotificationDto>(notification);
            return ServiceResult<SystemNotificationDto>.Successful(dto);
        }
    }
}