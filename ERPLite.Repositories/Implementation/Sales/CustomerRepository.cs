using ERPLite.Data.Context;
using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Extensions;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Sales;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Sales
{
    public class CustomerRepository : GenericRepository<Customer, int>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerWithOrdersAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Orders)
                    .ThenInclude(o => o.CreatedByUser)
                        .ThenInclude(u => u.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PaginationResult<Customer>> GetPagedWithOrdersAsync(
            string? search,
            int? departmentId,
            int pageNumber,
            int pageSize)
        {
            var query = _dbSet
                .AsNoTracking()
                .Include(c => c.Orders)
                    .ThenInclude(o => o.CreatedByUser)
                        .ThenInclude(u => u.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                query = query.Where(c =>
                    c.FullName.ToLower().Contains(cleanSearch) ||
                    c.Phone.Contains(cleanSearch));
            }

            if (departmentId.HasValue)
            {
                var deptId = departmentId.Value;
                query = query.Where(c => c.Orders.Any(o =>
                    o.CreatedByUser.Employee != null &&
                    o.CreatedByUser.Employee.DepartmentId == deptId));
            }

            return await query
                .OrderBy(c => c.FullName)
                .ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword)
        {
            keyword = keyword.Trim().ToLower();

            return await _dbSet
                .AsNoTracking()
                .Where(c =>
                    c.FullName.ToLower().Contains(keyword) || c.Phone.Contains(keyword))
                .ToListAsync();
        }

        public async Task<bool> CustomerExistsByPhoneAsync(string phone, int? excludedCustomerId = null)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var cleanPhone = phone.Trim();

            return await _dbSet.AnyAsync(c =>
                c.Phone == cleanPhone &&
                (!excludedCustomerId.HasValue || c.Id != excludedCustomerId.Value));
        }

        public async Task<bool> HasOrdersAsync(int customerId)
        {
            return await _context.Orders
                .AnyAsync(x => x.CustomerId == customerId);
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
