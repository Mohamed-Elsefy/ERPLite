using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.HR
{
    public interface IAttendanceRepository : IGenericRepository<Attendance, int>
    {
        Task<IEnumerable<Attendance>> GetEmployeeAttendanceAsync(int employeeId);

        Task<Attendance?> GetAttendanceByDateAsync(int employeeId, DateTime date);

        Task<bool> HasAttendanceTodayAsync(int employeeId);

        Task<IEnumerable<Attendance>> GetTodayAttendanceAsync();

        Task<IEnumerable<Attendance>> GetAttendanceByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<bool> HasAttendanceRecordsAsync(int employeeId);
    }
}
