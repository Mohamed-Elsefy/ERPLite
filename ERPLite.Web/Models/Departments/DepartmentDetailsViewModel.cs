using ERPLite.Services.DTOs.HR;

namespace ERPLite.Web.Models.Departments
{
    public class DepartmentDetailsViewModel
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EmployeesCount { get; set; }

        public IEnumerable<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
}