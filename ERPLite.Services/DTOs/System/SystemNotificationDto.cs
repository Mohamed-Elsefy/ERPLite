namespace ERPLite.Services.DTOs.System
{
    public class SystemNotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
