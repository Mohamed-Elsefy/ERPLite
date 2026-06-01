using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.HR
{
    public interface IAttendanceService
    {
        Task<ServiceResult> CheckInAsync(CheckInDto dto, string currentUserId);

        Task<ServiceResult> CheckOutAsync(CheckOutDto dto, string currentUserId);

        Task<ServiceResult<IEnumerable<AttendanceDto>>> GetEmployeeAttendanceAsync(int employeeId);

        Task<ServiceResult<IEnumerable<AttendanceDto>>> GetTodayAttendanceAsync();
    }
}