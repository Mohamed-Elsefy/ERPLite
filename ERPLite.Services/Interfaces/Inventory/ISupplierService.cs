using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Inventory
{
    public interface ISupplierService
    {
        Task<ServiceResult<IEnumerable<SupplierDto>>> GetAllAsync();

        Task<ServiceResult<SupplierDto>> GetByIdAsync(int id);

        Task<ServiceResult> CreateAsync(CreateSupplierDto dto, string currentUserId);

        Task<ServiceResult> UpdateAsync(UpdateSupplierDto dto, string currentUserId);

        Task<ServiceResult> DeleteAsync(int id, string currentUserId);
    }
}