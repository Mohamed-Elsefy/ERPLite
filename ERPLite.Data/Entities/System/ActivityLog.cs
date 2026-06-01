using ERPLite.Data.Entities.Identity;

namespace ERPLite.Data.Entities.System
{
    public class ActivityLog
    {
        public int Id { get; set; }

        public string Action { get; set; } = null!;

        public string EntityName { get; set; } = null!;

        public int EntityId { get; set; }     

        public string Description { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}