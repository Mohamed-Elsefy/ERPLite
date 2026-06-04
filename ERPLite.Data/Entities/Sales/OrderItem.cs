using ERPLite.Data.Entities.Inventory;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPLite.Data.Entities.Sales
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal SubTotal => Quantity * UnitPrice;
    }
}