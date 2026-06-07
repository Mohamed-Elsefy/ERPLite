using ERPLite.Services.DTOs.HR;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERPLite.Web.Models.Employees
{
    public class EditEmployeeViewModel
    {
        public UpdateEmployeeDto Employee { get; set; } = new UpdateEmployeeDto();
        public SelectList? DepartmentList { get; set; }
    }
}
