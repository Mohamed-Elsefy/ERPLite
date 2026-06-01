using ERPLite.Services.DTOs.Common;

namespace ERPLite.Services.DTOs.HR
{
    public class AttendanceDto : BaseDto
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public DateTime CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
