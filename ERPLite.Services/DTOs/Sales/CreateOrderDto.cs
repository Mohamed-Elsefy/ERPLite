namespace ERPLite.Services.DTOs.Sales
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;

        public List<CreateOrderItemDto> Items { get; set; }
            = new();
    }
}
