namespace ERPLite.Web.Areas.Admin.Models.Users
{
    public class UserIndexViewModel
    {
        public string? SearchTerm { get; set; }
        public List<UserListViewModel> Users { get; set; } = new();
    }

    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
    }
}
