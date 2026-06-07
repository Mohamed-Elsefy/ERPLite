using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.HR
{
    public interface IEmployeeRepository : IGenericRepository<Employee, int>
    {
        Task<Employee?> GetEmployeeWithAttendanceAsync(int employeeId);

        Task<Employee?> GetByEmailAsync(string email);

        Task<bool> ExistsByEmailAsync(string email, int? excludedEmployeeId = null);

        Task<Employee?> GetWithDepartmentAsync(int id);

        Task<IEnumerable<Employee>> GetAllWithDepartmentsAsync();

        Task<int> GetCountAsync();

        Task<int> GetActiveCountAsync();

        Task<int> GetActiveCountByDepartmentAsync(int? departmentId);
    }
}
