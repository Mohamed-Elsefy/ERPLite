using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Web.Areas.Employee.Models.Dashboard;

namespace ERPLite.Web.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Policy = "EmployeeOnly")]
    public class DashboardController : Controller
    {
        private readonly IAttendanceService _attendanceService;

        public DashboardController(IAttendanceService attendanceService)
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

            var attendanceResult = await _attendanceService.GetEmployeeAttendanceAsync(employeeId);

            if (!attendanceResult.Success || attendanceResult.Data == null)
            {
                ViewData["ErrorMessage"] = attendanceResult.Message ?? "Unable to load attendance data.";
                return View(new EmployeeDashboardViewModel());
            }

            var today = DateTime.Today;
            var todayAttendance = attendanceResult.Data.FirstOrDefault(a => a.Date.Date == today);

            var vm = new EmployeeDashboardViewModel
            {
                TodayAttendance = todayAttendance,
                RecentAttendances = attendanceResult.Data.OrderByDescending(a => a.Date).Take(10).ToList()
            };

            ViewData["PageTitle"] = "My Portal Dashboard";
            return View(vm);
        }
    }
}