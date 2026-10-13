namespace ERPLite.Shared.Constants
{
    /// <summary>
    /// Canonical notification channels and priorities. Use these in
    /// <c>CreateSystemNotificationAsync(type: ..., priority: ...)</c>.
    /// </summary>
    public static class NotificationTypes
    {
        public const string Sales = "Sales";

        public const string Inventory = "Inventory";

        public const string Finance = "Finance";

        public const string Procurement = "Procurement";

        public const string Catalog = "Catalog";
    }

    public static class NotificationPriorities
    {
        public const string Low = "Low";

        public const string Medium = "Medium";

        public const string High = "High";

        public const string Urgent = "Urgent";
    }
}
