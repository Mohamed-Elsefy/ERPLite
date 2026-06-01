using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.HR
{
    public interface IEmployeeService
    {
        Task<ServiceResult<IEnumerable<EmployeeDto>>> GetAllAsync();

        Task<ServiceResult<EmployeeDto>> GetByIdAsync(int id);

        Task<ServiceResult> CreateAsync(CreateEmployeeDto dto, string currentUserId);

        Task<ServiceResult> UpdateAsync(UpdateEmployeeDto dto, string currentUserId);

        Task<ServiceResult> DeleteAsync(int id, string currentUserId);
    }
}