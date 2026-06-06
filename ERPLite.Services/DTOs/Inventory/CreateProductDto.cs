using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERPLite.Services.DTOs.Inventory
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Please enter the product name")]
        public string Name { get; set; } = string.Empty;
        [DisplayName("Cost Price")]
        public decimal CostPrice { get; set; }
        [DisplayName("Selling Price")]

        public decimal SellingPrice { get; set; }

        public int QuantityInStock { get; set; }

        public int MinStockLevel { get; set; }

        public int CategoryId { get; set; }

        public int SupplierId { get; set; }
    }
}
