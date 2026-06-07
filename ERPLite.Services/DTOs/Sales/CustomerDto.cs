namespace ERPLite.Services.DTOs.Sales
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderDto> Orders { get; set; } = new();
        public List<OrderFinancialDto> FinancialPipelines { get; set; } = new();
    }
}
