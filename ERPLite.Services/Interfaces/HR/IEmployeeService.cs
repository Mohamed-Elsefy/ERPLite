using ERPLite.Services.DTOs.Common;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.HR
{
    public interface IEmployeeService
    {
        Task<ServiceResult<IEnumerable<EmployeeDto>>> GetAllAsync();

        Task<ServiceResult<PagedResponseDto<EmployeeDto>>> GetPagedAsync(PagedRequestDto request, int? departmentId);
        Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id);
        Task<ServiceResult<int>> CreateAsync(CreateEmployeeDto dto, string currentUserId);
        Task<ServiceResult> UpdateAsync(UpdateEmployeeDto dto, string currentUserId);
        Task<ServiceResult> DeleteAsync(int id, string currentUserId);
    }
}