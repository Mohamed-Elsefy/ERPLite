namespace ERPLite.Services.DTOs.Dashboard
{
    public class AttendanceDashboardDataDto
    {
        public int TotalEmployees { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public IEnumerable<AttendanceManagementDto> Records { get; set; } = new List<AttendanceManagementDto>();
    }
}
