namespace ERPLite.Web.Areas.Admin.Models.Users
{
    public class UserDetailsViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public DateTime AccountCreatedAt { get; set; }
        public int? EmployeeId { get; set; }
        public string? DepartmentName { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? HireDate { get; set; }
    }
}
