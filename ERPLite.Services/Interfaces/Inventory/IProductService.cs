using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Inventory
{
    public interface IProductService
    {
        Task<ServiceResult<IEnumerable<ProductDto>>> GetAllAsync();

        Task<ServiceResult<ProductDto>> GetByIdAsync(int id);

        Task<ServiceResult<IEnumerable<ProductDto>>> GetLowStockProductsAsync();

        Task<ServiceResult> CreateAsync(CreateProductDto dto, string currentUserId);

        Task<ServiceResult> UpdateAsync(UpdateProductDto dto, string currentUserId);

        Task<ServiceResult> DeleteAsync(int id, string currentUserId);
    }
}