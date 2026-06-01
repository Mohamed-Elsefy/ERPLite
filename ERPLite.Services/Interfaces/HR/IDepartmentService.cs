using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.HR
{
    public interface IDepartmentService
    {
        Task<ServiceResult<IEnumerable<DepartmentDto>>> GetAllAsync();

        Task<ServiceResult<DepartmentDto>> GetByIdAsync(int id);

        Task<ServiceResult> CreateAsync(CreateDepartmentDto dto, string currentUserId);

        Task<ServiceResult> UpdateAsync(UpdateDepartmentDto dto, string currentUserId);

        Task<ServiceResult> DeleteAsync(int id, string currentUserId);
    }
}