using ERPLite.Data.Context;
using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.HR;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.HR
{
    public class EmployeeRepository : GenericRepository<Employee, int>, IEmployeeRepository
    {
        public EmployeeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetEmployeeWithAttendanceAsync(int employeeId)
        {
            return await _dbSet
                .Include(e => e.Attendances)
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var cleanEmail = email.Trim();

            return await _dbSet.FirstOrDefaultAsync(x => x.Email == cleanEmail);
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludedEmployeeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var cleanEmail = email.Trim();

            return await _dbSet.AnyAsync(x =>
                x.Email == cleanEmail &&
                (!excludedEmployeeId.HasValue || x.Id != excludedEmployeeId.Value));
        }

        public async Task<Employee?> GetWithDepartmentAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Employee>> GetAllWithDepartmentsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(e => e.User)
                .CountAsync(e => e.User == null || e.User.LockoutEnd == null || e.User.LockoutEnd < DateTimeOffset.UtcNow);
        }

        public async Task<int> GetActiveCountByDepartmentAsync(int? departmentId)
        {
            var query = _context.Employees.AsNoTracking();

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            return await query.CountAsync();
        }
    }
}
