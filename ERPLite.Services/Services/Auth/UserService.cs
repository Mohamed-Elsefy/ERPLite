using AutoMapper;
using ERPLite.Data.Entities.HR;
using ERPLite.Data.Entities.Identity;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Auth;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Auth;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Services.Services.Auth
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IActivityLogService activityLogService,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(x => x.FullName)
                .ToListAsync();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var dto = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                dto.Role = roles.FirstOrDefault() ?? "";
                dto.IsLocked = await _userManager.IsLockedOutAsync(user);
                result.Add(dto);
            }

            return ServiceResult<IEnumerable<UserDto>>.Successful(result);
        }

        public async Task<ServiceResult<UserDto>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult<UserDto>.Failed("User not found.");

            var dto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "";
            dto.IsLocked = await _userManager.IsLockedOutAsync(user);

            return ServiceResult<UserDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateUserAsync(CreateUserDto dto, string currentUserId)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return ServiceResult.Failed("Email already exists in user accounts.");

            var emailExistsInEmployees = await _unitOfWork.Employees.ExistsByEmailAsync(dto.Email);
            if (emailExistsInEmployees)
                return ServiceResult.Failed("Email already exists in corporate employee records.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var employee = new Employee
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    HireDate = DateTime.Today,
                    Salary = 0,
                    DepartmentId = 1
                };

                await _unitOfWork.Employees.AddAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                var user = _mapper.Map<ApplicationUser>(dto);
                user.UserName = dto.Email;
                user.EmployeeId = employee.Id;

                var createResult = await _userManager.CreateAsync(user, dto.Password);

                if (!createResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return ServiceResult.Failed(string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }

                await _userManager.AddToRoleAsync(user, dto.Role);

                employee.UserId = user.Id;
                _unitOfWork.Employees.Update(employee);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                await _activityLogService.LogAsync(
                    userId: currentUserId,
                    action: "Create",
                    entityName: SystemModules.Users,
                    entityId: employee.Id,
                    description: $"System synchronization successful: Created account and matching HR Employee profile for '{user.FullName}' mapped to role '{dto.Role}'."
                );

                return ServiceResult.Successful("User account and matching Employee profile synchronized successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult.Failed($"An error occurred during synchronization: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateUserRoleAsync(UpdateUserRoleDto dto, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user is null)
                return ServiceResult.Failed("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, dto.Role);

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "UpdateRole",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"Modified application roles assignment for user '{user.FullName}'. Assigned new role: '{dto.Role}'."
            );

            return ServiceResult.Successful("Role updated successfully.");
        }

        public async Task<ServiceResult> LockUserAsync(string userId, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Failed("User not found.");

            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _userManager.UpdateAsync(user);

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Lock",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"Administrative block activated. Locked system user account: '{user.FullName}'."
            );

            return ServiceResult.Successful("User locked successfully.");
        }

        public async Task<ServiceResult> UnlockUserAsync(string userId, string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ServiceResult.Failed("User not found.");

            user.LockoutEnd = null;
            await _userManager.UpdateAsync(user);

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Unlock",
                entityName: SystemModules.Users,
                entityId: 0,
                description: $"Administrative block removed. Restored system access to user account: '{user.FullName}'."
            );

            return ServiceResult.Successful("User unlocked successfully.");
        }

        public async Task<ServiceResult<IEnumerable<UserDto>>> GetFilteredUsersAsync(string? search)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Email!.ToLower().Contains(search));
            }

            var users = await query.OrderBy(x => x.FullName).ToListAsync();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var dto = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                dto.Role = roles.FirstOrDefault() ?? "";
                dto.IsLocked = await _userManager.IsLockedOutAsync(user);
                result.Add(dto);
            }

            return ServiceResult<IEnumerable<UserDto>>.Successful(result);
        }
    }
}