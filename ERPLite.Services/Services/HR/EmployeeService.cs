using AutoMapper;
using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Common;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;
using FluentValidation;

namespace ERPLite.Services.Services.HR
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;
        private readonly IValidator<CreateEmployeeDto> _createValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;

        public EmployeeService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IActivityLogService activityLogService,
            IValidator<CreateEmployeeDto> createValidator,
            IValidator<UpdateEmployeeDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ServiceResult<IEnumerable<EmployeeDto>>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllWithDepartmentsAsync();
            var result = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return ServiceResult<IEnumerable<EmployeeDto>>.Successful(result);
        }

        public async Task<ServiceResult<PagedResponseDto<EmployeeDto>>> GetPagedAsync(PagedRequestDto request, int? departmentId)
        {
            var paged = await _unitOfWork.Employees.GetPagedWithDepartmentAsync(
                departmentId,
                request.SearchTerm,
                request.PageNumber,
                request.PageSize);

            return ServiceResult<PagedResponseDto<EmployeeDto>>.Successful(
                paged.ToPagedResponse<Employee, EmployeeDto>(_mapper));
        }

        public async Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetWithDepartmentAsync(id);
            if (employee is null)
                return ServiceResult<EmployeeDto>.Failed("Employee not found.");

            var dto = _mapper.Map<EmployeeDto>(employee);
            return ServiceResult<EmployeeDto>.Successful(dto);
        }

        public async Task<ServiceResult<int>> CreateAsync(CreateEmployeeDto dto, string currentUserId)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return ServiceResult<int>.Failed(validationResult.ToErrorString());

            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
            if (department is null)
                return ServiceResult<int>.Failed("Department not found.");

            var emailExists = await _unitOfWork.Employees.ExistsByEmailAsync(dto.Email);
            if (emailExists)
                return ServiceResult<int>.Failed("Email already exists.");

            var employee = _mapper.Map<Employee>(dto);

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: AuditActions.Create,
                entityName: SystemModules.Employees,
                entityId: employee.Id,
                description: $"Created employee profile for '{employee.FullName}' within department '{department.Name}'."
            );

            return ServiceResult<int>.Successful(employee.Id, "Employee created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateEmployeeDto dto, string currentUserId)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return ServiceResult.Failed(validationResult.ToErrorString());

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
                action: AuditActions.Update,
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

            _unitOfWork.Employees.Delete(employee);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: AuditActions.Delete,
                entityName: SystemModules.Employees,
                entityId: id,
                description: $"Permanently deleted employee entry '{employeeName}' from HR Domain."
            );

            return ServiceResult.Successful("Employee deleted successfully.");
        }
    }
}