using AutoMapper;
using ERPLite.Data.Entities.Identity;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Auth;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Services.Services.Auth
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IActivityLogService _activityLogService;

        public RoleService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IActivityLogService activityLogService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _activityLogService = activityLogService;
        }

        public async Task<ServiceResult<IEnumerable<string>>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();

            return ServiceResult<IEnumerable<string>>.Successful(roles);
        }

        public async Task<ServiceResult> AssignRoleAsync(string userId, string role, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Failed("User not found.");

            if (!await _roleManager.RoleExistsAsync(role))
                return ServiceResult.Failed("Role does not exist.");

            if (await _userManager.IsInRoleAsync(user, role))
                return ServiceResult.Failed("User already has this role.");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                return ServiceResult.Failed(
                    string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "AssignRole",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"Assigned application security role '{role}' to user account '{user.FullName}'."
            );

            return ServiceResult.Successful("Role assigned successfully.");
        }

        public async Task<ServiceResult> RemoveRoleAsync(string userId, string role, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Failed("User not found.");

            if (!await _userManager.IsInRoleAsync(user, role))
                return ServiceResult.Failed("User is not assigned to this role.");

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {
                return ServiceResult.Failed(
                    string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "RemoveRole",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"Revoked application security role '{role}' from user account '{user.FullName}'."
            );

            return ServiceResult.Successful("Role removed successfully.");
        }
    }
}