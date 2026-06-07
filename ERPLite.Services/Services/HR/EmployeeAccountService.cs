using ERPLite.Data.Entities.Identity;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ERPLite.Services.Services.HR
{
    public class EmployeeAccountService : IEmployeeAccountService
    {
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public EmployeeAccountService(
            IEmployeeService employeeService,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider)
        {
            _employeeService = employeeService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ServiceResult> CreateEmployeeWithAccountAsync(
            CreateEmployeeDto employeeDto,
            string? password,
            string? role,
            bool createAccount,
            string currentUserId)
        {
            if (createAccount)
            {
                if (string.IsNullOrWhiteSpace(password))
                    return ServiceResult.Failed("Password is required when creating a system account.");
                if (string.IsNullOrWhiteSpace(role))
                    return ServiceResult.Failed("Please assign a security role.");

                var userExists = await _userManager.FindByEmailAsync(employeeDto.Email);
                if (userExists != null)
                    return ServiceResult.Failed("This email is already registered to another user account.");
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var employeeResult = await _employeeService.CreateAsync(employeeDto, currentUserId);
                if (!employeeResult.Success)
                {
                    return ServiceResult.Failed(employeeResult.Message!);
                }

                int employeeId = employeeResult.Data; 

                if (createAccount)
                {
                    var newUser = new ApplicationUser
                    {
                        FullName = employeeDto.FullName,
                        Email = employeeDto.Email,
                        UserName = employeeDto.Email,
                        EmployeeId = employeeId,
                        CreatedAt = _dateTimeProvider.UtcNow
                    };

                    var createAccountResult = await _userManager.CreateAsync(newUser, password!);
                    if (!createAccountResult.Succeeded)
                    {
                        await _unitOfWork.RollbackTransactionAsync(); 
                        return ServiceResult.Failed("Personnel profile initiation aborted: Identity creation failed.");
                    }

                    await _userManager.AddToRoleAsync(newUser, role!);

                    var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
                    if (employee != null)
                    {
                        employee.UserId = newUser.Id;
                        _unitOfWork.Employees.Update(employee);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                await _unitOfWork.CommitTransactionAsync(); 
                return ServiceResult.Successful("Employee profile and correlated system identity established successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult.Failed($"An infrastructure breakdown occurred: {ex.Message}");
            }
        }
    }
}
