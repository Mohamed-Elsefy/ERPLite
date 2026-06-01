using ERPLite.Shared.Enums;

namespace ERPLite.Services.DTOs.Sales
{
    public class OrderDto
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public OrderPaymentStatus PaymentStatus { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
