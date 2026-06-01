using ERPLite.Shared.Enums;

namespace ERPLite.Services.DTOs.Sales
{
    public class OrderFinancialDto
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public OrderPaymentStatus PaymentStatus { get; set; }
    }
}
