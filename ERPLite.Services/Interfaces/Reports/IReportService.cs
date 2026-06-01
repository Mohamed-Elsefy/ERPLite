using ERPLite.Services.DTOs.Reports;

namespace ERPLite.Services.Interfaces.Reports
{
    public interface IReportService
    {
        Task<ReportResultDto<EmployeesReportDto>> GenerateEmployeesReportAsync();

        Task<ReportResultDto<AttendanceReportDto>> GenerateAttendanceReportAsync(DateTime from, DateTime to);

        Task<ReportResultDto<InventoryReportDto>> GenerateInventoryReportAsync();

        Task<ReportResultDto<SalesReportDto>> GenerateSalesReportAsync(DateTime from, DateTime to);

        Task<ReportResultDto<FinancialReportDto>> GenerateFinancialReportAsync();
    }
}
