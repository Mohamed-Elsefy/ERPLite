using ERPLite.Services.DTOs.Common;
using ERPLite.Services.DTOs.Users;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Users
{
    public interface IUserService
    {
        Task<ServiceResult<UserDto>> GetUserByIdAsync(string userId);

        Task<ServiceResult> CreateUserAsync(CreateUserDto dto, string currentUserId);

        Task<ServiceResult> UpdateUserRoleAsync(UpdateUserRoleDto dto, string currentUserId);

        Task<ServiceResult> GrantAccessToExistingEmployeeAsync(int employeeId, string password, string role, string currentUserId);

        Task<ServiceResult> LockUserAsync(string userId, string currentUserId);

        Task<ServiceResult> UnlockUserAsync(string userId, string currentUserId);

        Task<ServiceResult<PagedResponseDto<UserDto>>> GetPagedUsersAsync(string? search, PagedRequestDto request);

        Task<ServiceResult> RemoveUserAccessAsync(string userId, string currentUserId);
    }
}