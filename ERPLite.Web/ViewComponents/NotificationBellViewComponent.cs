using ERPLite.Services.Interfaces.System;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPLite.Web.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationBellViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = (UserClaimsPrincipal as ClaimsPrincipal)?.FindFirstValue(ClaimTypes.NameIdentifier);
            int unreadCount = 0;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _notificationService.GetUnreadCountAsync(userId);
                if (result.Success)
                {
                    unreadCount = result.Data;
                }
            }

            return View(unreadCount);
        }
    }
}
