using ERPLite.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Reports
{
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

        // GET: /Reports
        public IActionResult Index()
        {
            return View();
        }

        // PDF: /Reports/EmployeesPdf
        [Authorize(Policy = "AdminOnly")] 
        public async Task<IActionResult> EmployeesPdf()
        {
            var report = await _reportService.GenerateEmployeesReportAsync();
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"EmployeesReport_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // PDF: /Reports/InventoryPdf
        public async Task<IActionResult> InventoryPdf()
        {
            var report = await _reportService.GenerateInventoryReportAsync();
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"InventoryReport_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // PDF: /Reports/SalesPdf
        public async Task<IActionResult> SalesPdf(DateTime? from, DateTime? to)
        {
            var startDate = from ?? DateTime.Today.AddDays(-30);
            var endDate = to ?? DateTime.Today;

            var report = await _reportService.GenerateSalesReportAsync(startDate, endDate);
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"SalesReport_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf");
        }

        // PDF: /Reports/FinancialPdf
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> FinancialPdf()
        {
            var report = await _reportService.GenerateFinancialReportAsync();
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"FinancialReport_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // PDF: /Reports/AttendancePdf
        public async Task<IActionResult> AttendancePdf(DateTime? from, DateTime? to)
        {
            var startDate = from ?? DateTime.Today.AddDays(-7);
            var endDate = to ?? DateTime.Today;

            var report = await _reportService.GenerateAttendanceReportAsync(startDate, endDate);
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"AttendanceReport_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf");
        }
    }
}
