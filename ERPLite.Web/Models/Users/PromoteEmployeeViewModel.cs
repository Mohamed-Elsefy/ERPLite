using ERPLite.Shared.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Users
{
    public class PromoteEmployeeViewModel
    {
        [Required, Display(Name = "Select Unlinked Corporate Employee")]
        public int EmployeeId { get; set; }

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = Roles.Employee;

        public SelectList? UnlinkedEmployeesList { get; set; }
    }
}
