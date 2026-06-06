using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Users
{
    public class CreateUserViewModel
    {
        [Required, Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Manager";

        [Display(Name = "Is Internal Employee Link")]
        public bool IsEmployee { get; set; } = true;

        [Required, Range(0, 30000)]
        public decimal Salary { get; set; }

        [Required, Display(Name = "Assigned Department")]
        public int DepartmentId { get; set; }

        public SelectList? DepartmentList { get; set; }
    }
}
