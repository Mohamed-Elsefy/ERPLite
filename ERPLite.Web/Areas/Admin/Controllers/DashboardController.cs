using ERPLite.Services.Interfaces.Dashboard;
using ERPLite.Web.Areas.Admin.Models.Dashboard;
using ERPLite.Web.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")] 
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IAnalyticsService _analyticsService;

        public DashboardController(
            IDashboardService dashboardService,
            IAnalyticsService analyticsService)
        {
            _dashboardService = dashboardService;
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {
            var statisticsResult = await _dashboardService.GetStatisticsAsync();
            var analyticsResult = await _analyticsService.GetSalesAnalyticsAsync();

            if (!statisticsResult.Success || !analyticsResult.Success)
            {
                return View("Error");
            }

            var viewModel = new AdminDashboardViewModel
            {
                Statistics = statisticsResult.Data!,
                Analytics = analyticsResult.Data!
            };

            ViewData["PageTitle"] = "Admin Executive Dashboard";
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AttendanceHub()
        {
            var attendanceDataResult = await _dashboardService.GetAttendanceDashboardRecordsAsync(filterDepartmentId: null);

            if (!attendanceDataResult.Success || attendanceDataResult.Data == null)
            {
                return View("Error");
            }

            var viewModel = new ManagementDashboardViewModel
            {
                IsSuperAdmin = true,
                DepartmentName = "All Enterprise Departments",
                TotalEmployees = attendanceDataResult.Data.TotalEmployees,
                PresentToday = attendanceDataResult.Data.PresentCount,
                AbsentToday = attendanceDataResult.Data.AbsentCount,
                LateToday = attendanceDataResult.Data.LateCount,
                AttendanceRecords = attendanceDataResult.Data.Records
            };

            ViewData["PageTitle"] = "Enterprise Attendance Control";
            return View(viewModel);
        }
    }
}