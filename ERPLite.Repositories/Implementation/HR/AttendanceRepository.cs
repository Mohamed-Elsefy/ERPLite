using ERPLite.Data.Entities.HR;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.HR;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.HR
{
    public class AttendanceRepository : GenericRepository<Attendance, int>, IAttendanceRepository
    {
        public AttendanceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Attendance>> GetEmployeeAttendanceAsync(int employeeId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(a => a.Employee)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }


        public async Task<Attendance?> GetAttendanceByDateAsync(int employeeId,DateTime date)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.EmployeeId == employeeId &&
                    a.Date.Date == date.Date);
        }

        public async Task<bool> HasAttendanceTodayAsync(int employeeId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _dbSet
            .AnyAsync(a =>
                a.EmployeeId == employeeId &&
                a.Date >= today &&
                a.Date < tomorrow);
        }

        public async Task<IEnumerable<Attendance>> GetTodayAttendanceAsync()
        {
            var today = DateTime.Today;
            var tommorrow = today.AddDays(1);

            return await _dbSet
                .AsNoTracking()
                .Include(a => a.Employee)
                .Where(a => a.Date >= today && a.Date < tommorrow)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetAttendanceByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(a => a.Employee)
                .Where(a =>
                    a.Date >= startDate &&
                    a.Date <= endDate)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<bool> HasAttendanceRecordsAsync(int employeeId)
        {
            return await _dbSet
                .AnyAsync(x => x.EmployeeId == employeeId);
        }

        public async Task<IEnumerable<Attendance>> GetTodayAttendanceManagementAsync(int? departmentId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var query = _dbSet
                .AsNoTracking()
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Department)
                .Where(a => a.Date >= today && a.Date < tomorrow);

            if (departmentId.HasValue)
            {
                query = query.Where(a => a.Employee.DepartmentId == departmentId.Value);
            }

            return await query.ToListAsync();
        }
    }
}
