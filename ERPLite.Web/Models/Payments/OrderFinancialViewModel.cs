using ERPLite.Shared.Enums;

namespace ERPLite.Web.Models.Payments
{
    public class OrderFinancialViewModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public OrderPaymentStatus PaymentStatus { get; set; }
        public List<PaymentItemViewModel> Payments { get; set; } = new();
    }

    public class PaymentItemViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
    }

}
