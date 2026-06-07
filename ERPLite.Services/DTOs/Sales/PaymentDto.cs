using ERPLite.Shared.Enums;

namespace ERPLite.Services.DTOs.Sales
{
    public class PaymentDto
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public int OrderId { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
