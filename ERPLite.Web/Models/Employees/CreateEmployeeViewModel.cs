using ERPLite.Services.DTOs.HR;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Employees
{
    public class CreateEmployeeViewModel
    {
        public CreateEmployeeDto Employee { get; set; } = new CreateEmployeeDto { HireDate = DateTime.Today };
        public SelectList? DepartmentList { get; set; }

        [Display(Name = "Create System User Account?")]
        public bool CreateUserAccount { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The password must be at least 6 characters long.", MinimumLength = 6)]
        public string? Password { get; set; }

        public string? Role { get; set; }
    }
}
