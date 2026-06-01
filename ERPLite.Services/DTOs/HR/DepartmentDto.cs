using ERPLite.Services.DTOs.Common;

namespace ERPLite.Services.DTOs.HR
{
    public class DepartmentDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int EmployeesCount { get; set; }
    }
}
