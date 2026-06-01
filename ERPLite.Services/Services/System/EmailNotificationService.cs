using ERPLite.Services.DTOs.System;
using ERPLite.Services.Interfaces.System;
using Microsoft.Extensions.Logging;

namespace ERPLite.Services.Services.System
{
    public class EmailNotificationService : INotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(ILogger<EmailNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(NotificationDto notification)
        {
            // محاكاة إرسال البريد الإلكتروني عبر الـ Logs حالياً
            _logger.LogInformation("Email Sent To {Recipient} | Title: {Title} | Message: {Message}",
                notification.Recipient, notification.Title, notification.Message);

            await Task.CompletedTask;
        }
    }
}
