using ERPLite.Shared.Enums;

namespace ERPLite.Web.Models.Payments
{
    public class PaymentsIndexViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
