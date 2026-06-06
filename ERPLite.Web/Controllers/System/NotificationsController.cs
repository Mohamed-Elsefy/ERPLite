using ERPLite.Services.Interfaces.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERPLite.Web.Controllers.System
{
    [Authorize(Policy = "AllUsers")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: /Notifications
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _notificationService.GetUserNotificationsAsync(userId!);

            return View(result.Data);
        }

        // GET: /Notifications/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _notificationService.GetByIdAsync(id);
            if (!result.Success || result.Data == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _notificationService.GetByIdAsync(id);

            if (result.Success && result.Data?.UserId == userId)
            {
                await _notificationService.MarkAsReadAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
