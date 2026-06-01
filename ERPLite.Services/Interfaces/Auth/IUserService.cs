using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Auth
{
    public interface IUserService
    {
        Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync();

        Task<ServiceResult<UserDto>> GetUserByIdAsync(string userId);

        Task<ServiceResult> CreateUserAsync(CreateUserDto dto, string currentUserId);

        Task<ServiceResult> UpdateUserRoleAsync(UpdateUserRoleDto dto, string currentUserId);

        Task<ServiceResult> LockUserAsync(string userId, string currentUserId);

        Task<ServiceResult> UnlockUserAsync(string userId, string currentUserId);

        Task<ServiceResult<IEnumerable<UserDto>>> GetFilteredUsersAsync(string? search);
    }
}