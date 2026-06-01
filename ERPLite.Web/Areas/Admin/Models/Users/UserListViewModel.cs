namespace ERPLite.Web.Areas.Admin.Models.Users
{
    public class UserListViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsLocked { get; set; }
    }

    public class UserIndexViewModel
    {
        public string? SearchTerm { get; set; }
        public List<UserListViewModel> Users { get; set; } = new();
    }
}
