using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Inventory
{
    public interface IProductService
    {
        Task<ServiceResult<IEnumerable<ProductDto>>> GetAllAsync();

        Task<ServiceResult<ProductDto>> GetByIdAsync(int id);

        Task<ServiceResult<IEnumerable<ProductDto>>> GetLowStockProductsAsync();

        Task<ServiceResult> CreateAsync(CreateProductDto dto);

        Task<ServiceResult> UpdateAsync(UpdateProductDto dto);

        Task<ServiceResult> DeleteAsync(int id);
    }
}