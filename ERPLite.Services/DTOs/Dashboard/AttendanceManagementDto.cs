namespace ERPLite.Services.DTOs.Dashboard
{
    public class AttendanceManagementDto
    {
        public int AttendanceId { get; set; }
        public string EmployeeName { get; set; } = null!;
        public string DepartmentName { get; set; } = null!;
        public DateTime Date { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; } = null!;
    }
}
