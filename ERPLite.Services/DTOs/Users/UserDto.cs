namespace ERPLite.Services.DTOs.Users
{
    /// <summary>
    /// Identity keys are strings (<see cref="UserId"/>), so this DTO intentionally
    /// does not inherit the int-based <c>BaseDto</c> to avoid a misleading dual-ID shape.
    /// </summary>
    public class UserDto
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsLocked { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? EmployeeId { get; set; }
    }
}
