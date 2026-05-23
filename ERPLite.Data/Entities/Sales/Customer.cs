namespace ERPLite.Data.Entities.Sales
{
    public class Customer
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}