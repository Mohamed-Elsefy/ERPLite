using ERPLite.Services.Interfaces.Reports;
using ERPLite.Services.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPLite.Web.Controllers.Reports
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IExportService _exportService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReportsController(IReportService reportService, IExportService exportService, IDateTimeProvider dateTimeProvider)
        {
            _reportService = reportService;
            _exportService = exportService;
            _dateTimeProvider = dateTimeProvider;
        }

        // GET: /Reports
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Reports/EmployeesPdf
        public async Task<IActionResult> EmployeesPdf()
        {
            var report = await _reportService.GenerateEmployeesReportAsync();
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"EmployeesReport_{_dateTimeProvider.Now:yyyyMMdd}.pdf");
        }

        // GET: /Reports/InventoryPdf
        public async Task<IActionResult> InventoryPdf()
        {
            var report = await _reportService.GenerateInventoryReportAsync();
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"InventoryReport_{_dateTimeProvider.Now:yyyyMMdd}.pdf");
        }

        // GET: /Reports/SalesPdf
        public async Task<IActionResult> SalesPdf(DateTime? from, DateTime? to)
        {
            var today = _dateTimeProvider.Now.Date;
            var startDate = from ?? today.AddDays(-30);
            var endDate = to ?? today;

            var report = await _reportService.GenerateSalesReportAsync(startDate, endDate);
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"SalesReport_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf");
        }

        // GET: /Reports/FinancialPdf
        public async Task<IActionResult> FinancialPdf()
        {
            var report = await _reportService.GenerateFinancialReportAsync();
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"FinancialReport_{_dateTimeProvider.Now:yyyyMMdd}.pdf");
        }

        // GET: /Reports/AttendancePdf
        public async Task<IActionResult> AttendancePdf(DateTime? from, DateTime? to)
        {
            var today = _dateTimeProvider.Now.Date;
            var startDate = from ?? today.AddDays(-7);
            var endDate = to ?? today;

            var report = await _reportService.GenerateAttendanceReportAsync(startDate, endDate);
            var pdf = _exportService.ExportToPdf(report.ReportName, report.Data);
            return File(pdf, "application/pdf", $"AttendanceReport_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf");
        }
    }
}