using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERPLite.Web.ViewModels.Dashboard;
using ERPLite.Shared.Constants;

namespace ERPLite.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // Add dependency injection for IDashboardService when available

        [Authorize(Roles = Roles.Admin)]
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalEmployees = 150,
                TotalProducts = 320,
                TotalCustomers = 45,
                TotalOrders = 1024,
                TotalRevenue = 150000m,
                LowStockProducts = 12
            };
            return View(model);
        }

        [Authorize(Roles = Roles.Manager)]
        public IActionResult ManagerDashboard()
        {
            var model = new DashboardViewModel
            {
                TotalEmployees = 50,
                TotalProducts = 320,
                TotalOrders = 1024
            };
            return View(model);
        }

        [Authorize(Roles = Roles.Employee)]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }
    }
}
