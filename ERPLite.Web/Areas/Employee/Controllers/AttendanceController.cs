using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Web.Areas.Employee.Models.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Policy = "EmployeeOnly")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AttendanceController(
            IAttendanceService attendanceService,
            ICurrentUserService currentUser,
            IDateTimeProvider dateTimeProvider)
        {
            _attendanceService = attendanceService;
            _currentUser = currentUser;
            _dateTimeProvider = dateTimeProvider;
        }

        // GET: /Employee/Attendance
        public async Task<IActionResult> Index()
        {
            var employeeId = _currentUser.EmployeeId;
            if (!employeeId.HasValue)
            {
                return RedirectToAction("AccessDenied", "Auth", new { area = "" });
            }

            var result = await _attendanceService.GetEmployeeAttendanceAsync(employeeId.Value);
            var history = result.Data ?? new List<AttendanceDto>();

            var todayRecord = history.FirstOrDefault(a => a.Date.Date == _dateTimeProvider.Today);

            var viewModel = new AttendanceIndexViewModel
            {
                HistoryLog = history,
                HasCheckedInToday = todayRecord != null,
                HasCheckedOutToday = todayRecord?.CheckOutTime != null,
                TodayCheckInTime = todayRecord?.CheckInTime.ToString("hh:mm tt") ?? "--:--",
                TodayCheckOutTime = todayRecord?.CheckOutTime?.ToString("hh:mm tt") ?? "--:--"
            };

            return View(viewModel);
        }

        // POST: /Employee/Attendance/CheckIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn()
        {
            var employeeId = _currentUser.EmployeeId;
            if (!employeeId.HasValue)
            {
                return RedirectToAction("AccessDenied", "Auth", new { area = "" });
            }

            var result = await _attendanceService.CheckInAsync(
                new CheckInDto { EmployeeId = employeeId.Value },
                _currentUser.UserId!
            );

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // POST: /Employee/Attendance/CheckOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut()
        {
            var employeeId = _currentUser.EmployeeId;
            if (!employeeId.HasValue)
            {
                return RedirectToAction("AccessDenied", "Auth", new { area = "" });
            }

            var result = await _attendanceService.CheckOutAsync(
                new CheckOutDto { EmployeeId = employeeId.Value },
                _currentUser.UserId!
            );

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}