using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Auth
{
    public class RegisterViewModel
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

        [Required(ErrorMessage = "Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
