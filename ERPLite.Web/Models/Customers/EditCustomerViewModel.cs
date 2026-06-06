using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Customers
{
    public class EditCustomerViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Should be less than 100 characters.")]
        [Display(Name = "New Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Phone is not valid.")]
        [Display(Name = "New Phone")]
        public string Phone { get; set; } = string.Empty;
    }
}
