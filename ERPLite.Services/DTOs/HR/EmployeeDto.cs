using ERPLite.Services.DTOs.Common;

namespace ERPLite.Services.DTOs.HR
{
    public class EmployeeDto : BaseDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public string? UserId { get; set; }
    }
}
