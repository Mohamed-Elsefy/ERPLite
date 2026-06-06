using ERPLite.Services.DTOs.Dashboard;

namespace ERPLite.Web.Models.Employees
{
    public class EmployeeDetailsViewModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public bool HasSystemAccount => !string.IsNullOrEmpty(UserId);
        public string? UserId { get; set; }
        public string? SystemRole { get; set; }
        public bool IsAccountLocked { get; set; }
        public IEnumerable<AttendanceManagementDto> AttendanceLogs { get; set; } = new List<AttendanceManagementDto>();
    }
}
