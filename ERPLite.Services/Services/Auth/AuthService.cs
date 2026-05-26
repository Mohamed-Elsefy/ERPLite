using ERPLite.Data.Entities.Identity;
using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Exceptions;
using ERPLite.Services.Interfaces.Auth;
using Microsoft.AspNetCore.Identity;

namespace ERPLite.Services.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new InvalidCredentialsException();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);
            
            if (!result.Succeeded)
            {
                throw new InvalidCredentialsException();
            }

            return true;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
