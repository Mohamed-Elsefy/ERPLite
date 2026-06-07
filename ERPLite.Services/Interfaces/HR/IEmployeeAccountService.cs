using ERPLite.Services.DTOs.HR;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.HR
{
    public interface IEmployeeAccountService
    {
        Task<ServiceResult> CreateEmployeeWithAccountAsync(
            CreateEmployeeDto employeeDto,
            string? password,
            string? role,
            bool createAccount,
            string currentUserId);
    }
}
