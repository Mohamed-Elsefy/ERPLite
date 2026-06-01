namespace ERPLite.Services.DTOs.System
{
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        public string UserFullName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
