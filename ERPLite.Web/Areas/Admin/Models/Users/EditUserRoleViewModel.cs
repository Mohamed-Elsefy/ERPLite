using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Areas.Admin.Models.Users
{
    public class EditUserRoleViewModel
    {
        public string UserId { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Role is Required")]
        public string Role { get; set; } = null!;
    }
}
