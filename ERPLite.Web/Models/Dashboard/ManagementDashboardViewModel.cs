using ERPLite.Services.DTOs.Dashboard;

namespace ERPLite.Web.Models.Dashboard
{
    public class ManagementDashboardViewModel
    {
        public bool IsSuperAdmin { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        public int LateToday { get; set; }
        public IEnumerable<AttendanceManagementDto> AttendanceRecords { get; set; } = Enumerable.Empty<AttendanceManagementDto>();
    }
}
