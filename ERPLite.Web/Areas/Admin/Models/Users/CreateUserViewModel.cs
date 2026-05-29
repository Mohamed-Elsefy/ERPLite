using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Areas.Admin.Models.Users
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Employee Name is Required")]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Role is Required")]
        public string Role { get; set; } = null!;
    }
}
