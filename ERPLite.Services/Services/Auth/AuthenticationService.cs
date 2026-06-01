using ERPLite.Data.Entities.Identity;
using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Auth;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace ERPLite.Services.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IActivityLogService _activityLogService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IActivityLogService activityLogService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                user,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: true);

            if (!result.Succeeded)
                return ServiceResult.Failed("Invalid email or password.");

            await _activityLogService.LogAsync(
                userId: user.Id,
                action: "Login",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"User session initialized. Successfully logged into system pool: '{user.FullName}'."
            );

            return ServiceResult.Successful("Login successful.");
        }

        public async Task<ServiceResult> LogoutAsync(string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(currentUserId);
            var userName = user?.FullName ?? "Unknown User";

            await _signInManager.SignOutAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Logout",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"User session terminated cleanly. Logged out: '{userName}'."
            );

            return ServiceResult.Successful("Logged out successfully.");
        }

        public async Task<ServiceResult> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
                return ServiceResult.Failed("Email already exists.");

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return ServiceResult.Failed("Role does not exist.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
                return ServiceResult.Failed(string.Join(", ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, dto.Role);

            await _activityLogService.LogAsync(
                userId: user.Id,
                action: "Register",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"Self-registered user profile successfully for '{user.FullName}' under assigned baseline role '{dto.Role}'."
            );

            return ServiceResult.Successful("User created successfully.");
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

            var result = await _userManager.ChangePasswordAsync(
                    user,
                    currentPassword,
                    newPassword);

            if (!result.Succeeded)
                return ServiceResult.Failed(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "ChangePassword",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"Updated and rotated account security credentials for user profile target: '{user.FullName}'."
            );

            return ServiceResult.Successful("Password changed successfully.");
        }
    }
}