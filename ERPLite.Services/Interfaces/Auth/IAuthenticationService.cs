using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Helpers;
using System.Security.Claims;

namespace ERPLite.Services.Interfaces.Auth
{
    public interface IAuthenticationService
    {
        Task<ServiceResult> LoginAsync(LoginDto dto);
        Task<ServiceResult> LogoutAsync(string currentUserId);
        Task<ServiceResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, string currentUserId);
        Task<bool> IsSignedInAsync(ClaimsPrincipal user);
    }
}