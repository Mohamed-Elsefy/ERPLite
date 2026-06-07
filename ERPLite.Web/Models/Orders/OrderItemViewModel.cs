namespace ERPLite.Web.Models.Orders
{
    public class OrderItemViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal => Price * Quantity;
    }
}
