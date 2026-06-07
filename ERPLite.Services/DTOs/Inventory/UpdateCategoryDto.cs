namespace ERPLite.Services.DTOs.Inventory
{
    public class UpdateCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
