namespace ERPLite.Data.Entities.Sales
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public DateTime PaymentDate { get; set; }

        public string Status { get; set; } = null!;
    }
}