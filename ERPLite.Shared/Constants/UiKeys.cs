namespace ERPLite.Shared.Constants
{
    /// <summary>
    /// Canonical TempData / ViewData dictionary keys shared between
    /// controllers and Razor views. Prevents silent typos ("Sucess" bugs).
    /// </summary>
    public static class UiKeys
    {
        public const string Success = "Success";

        public const string Error = "Error";

        public const string PageTitle = "PageTitle";

        public const string Title = "Title";

        public const string ReturnUrl = "ReturnUrl";
    }
}
