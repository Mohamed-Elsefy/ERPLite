using AutoMapper;
using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.HR;
using ERPLite.Services.Interfaces.System;
using ERPLite.Shared.Constants;

namespace ERPLite.Services.Services.HR
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityLogService _activityLogService;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, IActivityLogService activityLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityLogService = activityLogService;
        }

        public async Task<ServiceResult<IEnumerable<DepartmentDto>>> GetAllAsync()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var result = _mapper.Map<IEnumerable<DepartmentDto>>(departments);

            return ServiceResult<IEnumerable<DepartmentDto>>.Successful(result);
        }

        public async Task<ServiceResult<DepartmentDto>> GetByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetWithEmployeesAsync(id);
            if (department is null)
                return ServiceResult<DepartmentDto>.Failed("Department not found.");

            var dto = _mapper.Map<DepartmentDto>(department);

            return ServiceResult<DepartmentDto>.Successful(dto);
        }

        public async Task<ServiceResult> CreateAsync(CreateDepartmentDto dto, string currentUserId)
        {
            var exists = await _unitOfWork.Departments.ExistsByNameAsync(dto.Name);
            if (exists)
                return ServiceResult.Failed("Department already exists.");

            var department = _mapper.Map<Department>(dto);

            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Create",
                entityName: SystemModules.Departments,
                entityId: department.Id,
                description: $"Created organizational department record: '{department.Name}'."
            );

            return ServiceResult.Successful("Department created successfully.");
        }

        public async Task<ServiceResult> UpdateAsync(UpdateDepartmentDto dto, string currentUserId)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(dto.Id);
            if (department is null)
                return ServiceResult.Failed("Department not found.");

            var exists = await _unitOfWork.Departments.ExistsByNameAsync(dto.Name, dto.Id);
            if (exists)
                return ServiceResult.Failed("Department name already exists.");

            _mapper.Map(dto, department);

            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Update",
                entityName: SystemModules.Departments,
                entityId: department.Id,
                description: $"Updated profile and metadata configuration for department: '{department.Name}'."
            );

            return ServiceResult.Successful("Department updated successfully.");
        }

        public async Task<ServiceResult> DeleteAsync(int id, string currentUserId)
        {
            var department = await _unitOfWork.Departments.GetWithEmployeesAsync(id);
            if (department is null)
                return ServiceResult.Failed("Department not found.");

            if (department.Employees.Any())
                return ServiceResult.Failed("Cannot delete department with employees.");

            var departmentName = department.Name;

            _unitOfWork.Departments.Delete(department);
            await _unitOfWork.SaveChangesAsync();

            await _activityLogService.LogAsync(
                userId: currentUserId,
                action: "Delete",
                entityName: SystemModules.Departments,
                entityId: id,
                description: $"Permanently deleted organizational department: '{departmentName}'."
            );

            return ServiceResult.Successful("Department deleted successfully.");
        }
    }
}