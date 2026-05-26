using ERPLite.Services.DTOs.Auth;

namespace ERPLite.Services.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
    }
}
