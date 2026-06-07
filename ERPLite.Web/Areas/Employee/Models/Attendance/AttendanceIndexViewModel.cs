using ERPLite.Services.DTOs.HR;

namespace ERPLite.Web.Areas.Employee.Models.Attendance
{
    public class AttendanceIndexViewModel
    {
        public IEnumerable<AttendanceDto> HistoryLog { get; set; } = new List<AttendanceDto>();
        public bool HasCheckedInToday { get; set; }
        public bool HasCheckedOutToday { get; set; }
        public string? TodayCheckInTime { get; set; }
        public string? TodayCheckOutTime { get; set; }
    }
}
