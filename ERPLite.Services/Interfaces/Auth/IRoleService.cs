using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Auth
{
    public interface IRoleService
    {
        Task<ServiceResult<IEnumerable<string>>> GetAllRolesAsync();

        Task<ServiceResult> AssignRoleAsync(string userId, string role, string currentUserId);

        Task<ServiceResult> RemoveRoleAsync(string userId, string role, string currentUserId);
    }
}