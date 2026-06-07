using ERPLite.Data.Entities.Identity;
using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Auth;
using ERPLite.Services.Interfaces.System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ERPLite.Services.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IActivityLogService _activityLogService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IActivityLogService activityLogService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _activityLogService = activityLogService;
        }

        public async Task<ServiceResult> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
                return ServiceResult.Failed("Invalid email or password.");

            if (await _userManager.IsLockedOutAsync(user))
                return ServiceResult.Failed("Account is locked.");

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: true);

            if (!result.Succeeded)
                return ServiceResult.Failed("Invalid email or password.");

            await _activityLogService.LogAsync(
                userId: user.Id,
                action: "Login",
                entityName: "Users",
                entityId: 0,
                description: "User logged in."
            );

            return ServiceResult.Successful("Login successful.");
        }

        public async Task<ServiceResult> LogoutAsync(string currentUserId)
        {
            await _signInManager.SignOutAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Logout",
                entityName: "Users",
                entityId: 0,
                description: "User logged out."
            );

            return ServiceResult.Successful("Logged out successfully.");
        }

        public async Task<ServiceResult> ChangePasswordAsync(
            string userId,
            string currentPassword,
            string newPassword,
            string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return ServiceResult.Failed("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
                return ServiceResult.Failed(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "ChangePassword",
                entityName: "Users",
                entityId: 0,
                description: "Password changed."
            );

            return ServiceResult.Successful("Password changed successfully.");
        }

        public async Task<bool> IsSignedInAsync(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }
    }
}