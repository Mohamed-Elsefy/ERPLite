using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Web.Areas.Employee.Models.Attendance;

namespace ERPLite.Web.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Policy = "EmployeeOnly")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        public async Task<IActionResult> Index()
        {
            var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
            {
                return RedirectToAction("AccessDenied", "Auth", new { area = "" });
            }

            var result = await _attendanceService.GetEmployeeAttendanceAsync(employeeId);
            var history = result.Data ?? new List<AttendanceDto>();

            var todayLocal = DateTime.Today;
            var todayRecord = history.FirstOrDefault(a => a.Date.Date == todayLocal);

            var viewModel = new AttendanceIndexViewModel
            {
                HistoryLog = history,
                HasCheckedInToday = todayRecord != null,
                HasCheckedOutToday = todayRecord?.CheckOutTime != null,
                TodayCheckInTime = todayRecord?.CheckInTime.ToString("hh:mm tt"),
                TodayCheckOutTime = todayRecord?.CheckOutTime?.ToString("hh:mm tt")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
            {
                return RedirectToAction("AccessDenied", "Auth", new { area = "" });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _attendanceService.CheckInAsync(new CheckInDto { EmployeeId = employeeId }, currentUserId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;
            if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
            {
                return RedirectToAction("AccessDenied", "Auth", new { area = "" });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _attendanceService.CheckOutAsync(new CheckOutDto { EmployeeId = employeeId }, currentUserId);

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}