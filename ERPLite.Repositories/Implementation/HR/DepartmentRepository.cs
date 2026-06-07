using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.HR;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.HR
{
    public class DepartmentRepository : GenericRepository<Department, int>, IDepartmentRepository
    {
        public DepartmentRepository(Data.Context.AppDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludedDepartmentId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var cleanName = name.Trim();

            return await _dbSet.AnyAsync(x =>
                x.Name == cleanName &&
                (!excludedDepartmentId.HasValue || x.Id != excludedDepartmentId.Value));
        }

        public async Task<Department?> GetWithEmployeesAsync(int id)
        {
            return await _dbSet
                .Include(x => x.Employees)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Department>> GetAllWithEmployeesAsync()
        {
            return await _dbSet
                .Include(x => x.Employees)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
