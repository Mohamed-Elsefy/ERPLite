namespace ERPLite.Data.Entities
{
    public abstract class BaseEntity
    {
        public DateTimeOffset CreatedAtUtc { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTimeOffset? LastModifiedUtc { get; set; }

        public string? LastModifiedBy { get; set; }
    }
}
