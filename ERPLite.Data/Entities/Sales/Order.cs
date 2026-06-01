using ERPLite.Data.Entities.Identity;
using ERPLite.Shared.Enums;

namespace ERPLite.Data.Entities.Sales
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public string CreatedByUserId { get; set; } = null!;

        public ApplicationUser CreatedByUser { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public OrderPaymentStatus PaymentStatus { get; set; } = OrderPaymentStatus.Unpaid;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}