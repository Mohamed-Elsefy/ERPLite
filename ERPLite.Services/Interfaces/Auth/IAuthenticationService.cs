using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<ServiceResult> LoginAsync(LoginDto dto);

        Task<ServiceResult> LogoutAsync(string currentUserId);

        Task<ServiceResult> RegisterAsync(RegisterDto dto);

        Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, string currentUserId);
    }
}