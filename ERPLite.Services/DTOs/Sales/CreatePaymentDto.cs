using ERPLite.Shared.Enums;

namespace ERPLite.Services.DTOs.Sales
{
    public class CreatePaymentDto
    {
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}
