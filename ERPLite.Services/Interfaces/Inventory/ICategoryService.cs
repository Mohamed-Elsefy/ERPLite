using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Inventory
{
    public interface ICategoryService
    {
        Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync();

        Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);

        Task<ServiceResult> CreateAsync(CreateCategoryDto dto, string currentUserId);

        Task<ServiceResult> UpdateAsync(UpdateCategoryDto dto, string currentUserId);

        Task<ServiceResult> DeleteAsync(int id, string currentUserId);
    }
}