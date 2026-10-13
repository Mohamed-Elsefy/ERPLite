using ERPLite.Services.DTOs.HR;
using ERPLite.Web.Models.Common;

namespace ERPLite.Web.Models.Employees
{
    public class EmployeeIndexViewModel
    {
        public IEnumerable<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
        public string? SearchTerm { get; set; }
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
