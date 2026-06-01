namespace ERPLite.Services.DTOs.Inventory
{
    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; }
    }
}
