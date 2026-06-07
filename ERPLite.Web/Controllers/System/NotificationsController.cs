using ERPLite.Services.Interfaces.System;
using ERPLite.Services.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.System
{
    [Authorize(Policy = "AuthenticatedUser")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ICurrentUserService _currentUser;

        public NotificationsController(INotificationService notificationService, ICurrentUserService currentUser)
        {
            _notificationService = notificationService;
            _currentUser = currentUser;
        }

        // GET: /Notifications
        public async Task<IActionResult> Index()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId)) return Forbid();

            var result = await _notificationService.GetUserNotificationsAsync(userId);
            return View(result.Data);
        }

        // GET: /Notifications/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _notificationService.GetByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            var userId = _currentUser.UserId;
            if (result.Data.UserId != userId) return Forbid();

            if (!result.Data.IsRead)
            {
                await _notificationService.MarkAsReadAsync(id);
            }

            return View(result.Data);
        }

        // POST: /Notifications/MarkAsRead/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = _currentUser.UserId;
            var result = await _notificationService.GetByIdAsync(id);

            if (result.Success && result.Data?.UserId == userId)
            {
                await _notificationService.MarkAsReadAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}