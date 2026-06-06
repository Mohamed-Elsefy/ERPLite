using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERPLite.Services.DTOs.Inventory
{
    public class StockOperationDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue,
        ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
