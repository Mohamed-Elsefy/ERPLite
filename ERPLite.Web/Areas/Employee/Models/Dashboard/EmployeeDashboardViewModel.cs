using ERPLite.Services.DTOs.HR;

namespace ERPLite.Web.Areas.Employee.Models.Dashboard
{
    public class EmployeeDashboardViewModel
    {
        public AttendanceDto? TodayAttendance { get; set; }
        public List<AttendanceDto> RecentAttendances { get; set; } = new();
    }
}
