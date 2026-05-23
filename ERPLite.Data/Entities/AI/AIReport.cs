using ERPLite.Data.Entities.Identity;

namespace ERPLite.Data.Entities.AI
{
    public class AIReport
    {
        public int Id { get; set; }

        public string Type { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public string GeneratedByUserId { get; set; } = null!;

        public ApplicationUser GeneratedByUser { get; set; } = null!;

        public string InputSummary { get; set; } = null!;

        public string AIResponse { get; set; } = null!;

        public int? RelatedEntityId { get; set; }

        public string? RelatedEntityType { get; set; }
    }
}