using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ERPLite.Services.Interfaces.Dashboard;
using ERPLite.Web.Areas.Admin.Models.Dashboard;

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
            var statistics = await _dashboardService.GetStatisticsAsync();
            var analytics = await _analyticsService.GetSalesAnalyticsAsync();

            var viewModel = new AdminDashboardViewModel
            {
                Statistics = statistics,
                Analytics = analytics
            };

            return View(viewModel);
        }
    }
}