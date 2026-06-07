using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERPLite.Web.Models.Products
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product identifier name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit procurement price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive asset valuation.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Initial inventory tracking quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot rest below zero stock level.")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Minimum safety buffer stock level is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimum stock level cannot be negative.")]
        public int MinStockLevel { get; set; }

        [Required(ErrorMessage = "You must select a categorical classification.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "You must assign an approved procurement supplier.")]
        public int SupplierId { get; set; }

        public bool IsEditMode { get; set; }

        public IEnumerable<SelectListItem> CategoriesList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> SuppliersList { get; set; } = new List<SelectListItem>();
    }
}
