using ERPLite.Services.Interfaces.Dashboard;
using ERPLite.Web.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Policy = "ManagerOnly")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var statisticsResult = await _dashboardService.GetStatisticsAsync();

            if (!statisticsResult.Success || statisticsResult.Data == null)
            {
                return View("Error");
            }

            var vm = new ManagerDashboardViewModel
            {
                Statistics = statisticsResult.Data
            };

            ViewData["PageTitle"] = "Manager Operational Dashboard";
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AttendanceHub()
        {
            var departmentIdClaim = User.FindFirst("DepartmentId")?.Value;

            if (string.IsNullOrEmpty(departmentIdClaim) || !int.TryParse(departmentIdClaim, out int departmentId))
            {
                return RedirectToAction("AccessDenied", "Auth", new { area = "" });
            }

            var attendanceDataResult = await _dashboardService.GetAttendanceDashboardRecordsAsync(filterDepartmentId: departmentId);

            if (!attendanceDataResult.Success || attendanceDataResult.Data == null)
            {
                return View("Error");
            }

            var viewModel = new ManagementDashboardViewModel
            {
                IsSuperAdmin = false,
                DepartmentName = attendanceDataResult.Data.Records.FirstOrDefault()?.DepartmentName ?? "Assigned Department",
                TotalEmployees = attendanceDataResult.Data.TotalEmployees,
                PresentToday = attendanceDataResult.Data.PresentCount,
                AbsentToday = attendanceDataResult.Data.AbsentCount,
                LateToday = attendanceDataResult.Data.LateCount,
                AttendanceRecords = attendanceDataResult.Data.Records
            };

            ViewData["PageTitle"] = "Department Attendance Hub";
            return View(viewModel);
        }
    }
}