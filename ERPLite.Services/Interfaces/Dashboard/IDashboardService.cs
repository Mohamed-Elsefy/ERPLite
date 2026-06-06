using ERPLite.Services.DTOs.Dashboard;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<ServiceResult<DashboardStatisticsDto>> GetStatisticsAsync();

        Task<ServiceResult<AttendanceDashboardDataDto>> GetAttendanceDashboardRecordsAsync(int? filterDepartmentId);

        Task<ServiceResult<IEnumerable<AttendanceManagementDto>>> GetEmployeeHistoryAsync(int employeeId);
    }
}
