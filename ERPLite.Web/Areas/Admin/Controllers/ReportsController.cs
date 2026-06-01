using ERPLite.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "ManagerOnly")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IExportService _exportService;

        public ReportsController(IReportService reportService, IExportService exportService)
        {
            _reportService = reportService;
            _exportService = exportService;
        }

        public IActionResult Index()
        {
            return View();
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
