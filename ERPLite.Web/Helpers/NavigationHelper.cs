namespace ERPLite.Web.Helpers
{
    public static class NavigationHelper
    {
        public static string IsActive(string currentController, string targetController)
        {
            return string.Equals(currentController, targetController, System.StringComparison.OrdinalIgnoreCase)
                ? "active"
                : string.Empty;
        }
    }
}
