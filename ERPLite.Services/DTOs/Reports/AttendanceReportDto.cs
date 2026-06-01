using ERPLite.Services.DTOs.HR;

namespace ERPLite.Services.DTOs.Reports
{
    public class AttendanceReportDto
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int TotalRecords { get; set; }

        public int PresentCount { get; set; }

        public int LateCount { get; set; }

        public List<AttendanceDto> Records { get; set; } = new();
    }
}
