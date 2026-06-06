using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Inventory
{
    public interface ICategoryService
    {
        Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllAsync();

        Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);

        Task<ServiceResult> CreateAsync(CreateCategoryDto dto);

        Task<ServiceResult> UpdateAsync(UpdateCategoryDto dto);

        Task<ServiceResult> DeleteAsync(int id);

        Task<ServiceResult<IEnumerable<CategoryDto>>> SearchAsync(
            string search);
    }
}