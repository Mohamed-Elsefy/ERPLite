using ERPLite.Data.Entities.Identity;

namespace ERPLite.Data.Entities.System
{
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Priority { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;
    }
}