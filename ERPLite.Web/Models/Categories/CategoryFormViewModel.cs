using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Categories
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is strictly required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        public bool IsEditMode { get; set; }
    }
}
