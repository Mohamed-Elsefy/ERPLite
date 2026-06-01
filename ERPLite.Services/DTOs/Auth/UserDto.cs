using ERPLite.Services.DTOs.Common;

namespace ERPLite.Services.DTOs.Auth
{
    public class UserDto : BaseDto
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public bool IsLocked { get; set; }
    }
}
