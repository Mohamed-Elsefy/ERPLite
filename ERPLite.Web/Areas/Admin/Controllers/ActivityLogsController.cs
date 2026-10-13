using ERPLite.Services.DTOs.Common;
using ERPLite.Services.DTOs.System;
using ERPLite.Services.Interfaces.System;
using ERPLite.Web.Areas.Admin.Models.System;
using ERPLite.Web.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERPLite.Shared.Constants;

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
        public async Task<IActionResult> Index(string? selectedUser, string? selectedModule, int page = 1, int pageSize = 10)
        {
            var request = new PagedRequestDto { PageNumber = page, PageSize = pageSize };
            var result = await _activityLogService.GetPagedAsync(request, selectedUser, selectedModule);
            if (!result.Success || result.Data == null)
            {
                TempData[UiKeys.Error] = "Unable to retrieve system audit logs.";
                return View(new ActivityLogsIndexViewModel());
            }

            var paged = result.Data;
            var filters = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(selectedUser))
                filters["selectedUser"] = selectedUser;
            if (!string.IsNullOrWhiteSpace(selectedModule))
                filters["selectedModule"] = selectedModule;

            var vm = new ActivityLogsIndexViewModel
            {
                Logs = paged.Items,
                SelectedUser = selectedUser,
                SelectedModule = selectedModule,
                Pagination = PaginationViewModel.FromPagedResponse(paged, filters)
            };

            return View(vm);
        }

        // GET: /Admin/ActivityLogs/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var result = await _activityLogService.GetLogByIdAsync(id);
            if (!result.Success || result.Data == null)
                return NotFound();

            var vm = new ActivityLogDetailsViewModel
            {
                Log = result.Data
            };

            return View(vm);
        }
    }
}