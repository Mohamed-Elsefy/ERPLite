using ERPLite.Services.Interfaces.System;
using ERPLite.Web.Areas.Admin.Models.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [Authorize(Policy = "AdminOnly")]
    public class ActivityLogsController : Controller
    {
        private readonly IActivityLogService _activityLogService;

        public ActivityLogsController(IActivityLogService activityLogService)
        {
            _activityLogService = activityLogService;
        }

        // GET: /Admin/ActivityLogs
        public async Task<IActionResult> Index(string? selectedUser, string? selectedModule)
        {
            var result = await _activityLogService.GetRecentLogsAsync();
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Unable to retrieve system operational log vectors.";
                return View(new ActivityLogsIndexViewModel());
            }

            var logs = result.Data;

            if (!string.IsNullOrEmpty(selectedUser))
            {
                logs = logs.Where(l => l.UserFullName.Contains(selectedUser, StringComparison.OrdinalIgnoreCase) ||
                                       l.UserId.Equals(selectedUser, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(selectedModule))
            {
                logs = logs.Where(l => l.EntityName.Equals(selectedModule, StringComparison.OrdinalIgnoreCase));
            }

            var vm = new ActivityLogsIndexViewModel
            {
                Logs = logs.ToList(),
                SelectedUser = selectedUser,
                SelectedModule = selectedModule
            };

            return View(vm);
        }

        // GET: /Admin/ActivityLogs/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _activityLogService.GetRecentLogsAsync();
            if (!result.Success || result.Data == null)
                return NotFound();

            var targetLog = result.Data.FirstOrDefault(l => l.Id == id);
            if (targetLog == null)
                return NotFound();

            var vm = new ActivityLogDetailsViewModel
            {
                Log = targetLog
            };

            return View(vm);
        }
    }
}
