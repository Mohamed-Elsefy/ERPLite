using ERPLite.Services.Interfaces.Reports;
using ERPLite.Services.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ManagerOnly")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IExportService _exportService;
        private readonly IAnalyticsService _analyticsService;

        public ReportsController(IReportService reportService, IExportService exportService, IAnalyticsService analyticsService)
        {
            _reportService = reportService;
            _exportService = exportService;
            _analyticsService = analyticsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Employees()
        {
            var report = await _reportService.GenerateEmployeesReportAsync();
            return View(report.Data);
        }

        public async Task<IActionResult> Attendance(DateTime? from, DateTime? to)
        {
            var start = from ?? DateTime.Today.AddDays(-7);
            var end = to ?? DateTime.Today;
            var report = await _reportService.GenerateAttendanceReportAsync(start, end);
            return View(report.Data);
        }

        public async Task<IActionResult> Inventory()
        {
            var report = await _reportService.GenerateInventoryReportAsync();
            return View(report.Data);
        }

        public async Task<IActionResult> Sales(DateTime? from, DateTime? to)
        {
            var start = from ?? DateTime.Today.AddDays(-30);
            var end = to ?? DateTime.Today;
            var report = await _reportService.GenerateSalesReportAsync(start, end);
            return View(report.Data);
        }

        public async Task<IActionResult> Financial()
        {
            var report = await _reportService.GenerateFinancialReportAsync();
            return View(report.Data);
        }

        public async Task<IActionResult> Analytics()
        {
            var analytics = await _analyticsService.GetSalesAnalyticsAsync();
            return View(analytics);
        }

        public async Task<IActionResult> EmployeesPdf()
        {
            var report = await _reportService.GenerateEmployeesReportAsync();

            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);

            return File(pdf, "application/pdf", $"EmployeesReport_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> InventoryPdf()
        {
            var report = await _reportService.GenerateInventoryReportAsync();

            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);

            return File(pdf, "application/pdf", $"InventoryReport_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> SalesPdf(DateTime? from, DateTime? to)
        {
            var startDate = from ?? DateTime.Today.AddDays(-30);
            var endDate = to ?? DateTime.Today;

            var report = await _reportService.GenerateSalesReportAsync(startDate, endDate);

            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);

            return File(pdf, "application/pdf", $"SalesReport_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> FinancialPdf()
        {
            var report = await _reportService.GenerateFinancialReportAsync();

            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);

            return File(pdf, "application/pdf", $"FinancialReport_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
