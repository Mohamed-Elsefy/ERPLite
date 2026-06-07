using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Sales
{
    public interface ICustomerService
    {
        Task<ServiceResult<IEnumerable<CustomerDto>>> GetAllAsync();

        Task<ServiceResult<CustomerDto>> GetByIdAsync(int id);

        Task<ServiceResult> CreateAsync(CreateCustomerDto dto, string currentUserId);

        Task<ServiceResult> UpdateAsync(UpdateCustomerDto dto, string currentUserId);

        Task<ServiceResult> DeleteAsync(int id, string currentUserId);

        Task<ServiceResult<IEnumerable<CustomerDto>>> SearchAsync(string keyword);
    }
}