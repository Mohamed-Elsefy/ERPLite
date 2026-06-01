using ERPLite.Services.DTOs.HR;

namespace ERPLite.Services.DTOs.Reports
{
    public class EmployeesReportDto
    {
        public int TotalEmployees { get; set; }

        public int ActiveEmployees { get; set; }

        public decimal TotalSalaries { get; set; }

        public List<EmployeeDto> Employees { get; set; } = new();
    }
}
