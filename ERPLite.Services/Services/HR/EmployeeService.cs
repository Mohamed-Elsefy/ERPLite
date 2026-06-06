using AutoMapper;
using ERPLite.Data.Entities.HR;
using ERPLite.Data.Entities.Identity;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace ERPLite.Services.Services.HR
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _userManager = userManager;
        }

        public async Task<ServiceResult<IEnumerable<EmployeeDto>>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllWithDepartmentsAsync();

            var result = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return ServiceResult<IEnumerable<EmployeeDto>>.Successful(result);
        }

        public async Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetWithDepartmentAsync(id);
            if (employee is null)
                return ServiceResult<EmployeeDto>.Failed("Employee not found.");

            var dto = _mapper.Map<EmployeeDto>(employee);
            return ServiceResult<EmployeeDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateAsync(CreateEmployeeDto dto, string currentUserId)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
            if (department is null)
                return ServiceResult.Failed("Department not found.");

            var emailExists = await _unitOfWork.Employees.ExistsByEmailAsync(dto.Email);
            if (emailExists)
                return ServiceResult.Failed("Email already exists.");

            if (dto.HireDate > DateTime.Today)
                return ServiceResult.Failed("Invalid hire date.");

            var employee = _mapper.Map<Employee>(dto);

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Create",
                entityName: SystemModules.Employees,
                entityId: employee.Id,
                description: $"Created employee profile for '{employee.FullName}' within department '{department.Name}'."
            );

            return ServiceResult.Successful("Employee created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateEmployeeDto dto, string currentUserId)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.Id);
            if (employee is null)
                return ServiceResult.Failed("Employee not found.");

            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
            if (department is null)
                return ServiceResult.Failed("Department not found.");

            var emailExists = await _unitOfWork.Employees.ExistsByEmailAsync(dto.Email, dto.Id);
            if (emailExists)
                return ServiceResult.Failed("Email already exists.");

            _mapper.Map(dto, employee);

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Update",
                entityName: SystemModules.Employees,
                entityId: employee.Id,
                description: $"Updated employee data details for: '{employee.FullName}'."
            );

            return ServiceResult.Successful("Employee updated successfully.");
        }

        public async Task<ServiceResult> DeleteAsync(int id, string currentUserId)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee is null)
                return ServiceResult.Failed("Employee not found.");

            var employeeName = employee.FullName;
            var linkedUserId = employee.UserId;

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Employees.Delete(employee);
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(linkedUserId))
                {
                    var user = await _userManager.FindByIdAsync(linkedUserId);
                    if (user != null)
                    {
                        var userDeleteResult = await _userManager.DeleteAsync(user);
                        if (!userDeleteResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            return ServiceResult.Failed("Failed to purge linked system user identity.");
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                await _activityLogService.LogAsync(
                    userId: currentUserId,
                    action: "Delete",
                    entityName: SystemModules.Employees,
                    entityId: id,
                    description: $"Permanently deleted employee entry '{employeeName}'. Correlated attendance records preserved and unlinked successfully."
                );

                return ServiceResult.Successful("Employee deleted successfully. Historical attendance data preserved.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResult.Failed($"An unexpected error occurred during database update: {ex.Message}");
            }
        }
    }
}