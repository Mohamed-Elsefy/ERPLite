using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.HR
{
    public interface IDepartmentRepository : IGenericRepository<Department, int>
    {
        Task<bool> ExistsByNameAsync(string name, int? excludedDepartmentId = null);

        Task<Department?> GetWithEmployeesAsync(int id);

        Task<int> GetCountAsync();
    }
}
