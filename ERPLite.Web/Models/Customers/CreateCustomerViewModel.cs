using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Customers
{
    public class CreateCustomerViewModel
    {
        [Required(ErrorMessage = "Enter customer full name")]
        [StringLength(100, ErrorMessage = "Should be less than 100 characters")]
        [Display(Name = "Customer Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Phone number is not valid")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;
    }
}
