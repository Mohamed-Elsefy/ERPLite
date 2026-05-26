using ERPLite.Data.Enums;

namespace ERPLite.Data.Entities.Sales
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }
    }
}