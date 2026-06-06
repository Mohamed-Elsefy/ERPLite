using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Suppliers
{
    public class SupplierFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier corporate name is strictly required.")]
        [StringLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string? Address { get; set; }

        public bool IsEditMode { get; set; }
    }
}
