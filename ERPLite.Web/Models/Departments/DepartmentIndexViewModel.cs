using ERPLite.Services.DTOs.HR;

namespace ERPLite.Web.Models.Departments
{
    public class DepartmentIndexViewModel
    {
        public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
        public string? SearchTerm { get; set; }
    }
}
