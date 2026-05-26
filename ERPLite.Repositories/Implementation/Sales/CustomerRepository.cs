using ERPLite.Data.Entities.Sales;
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

        // =====================================
        // Active Customers
        // =====================================

        public async Task<IEnumerable<Customer>>
            GetActiveCustomersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================
        // Customer With Orders
        // =====================================

        public async Task<Customer?>
            GetCustomerWithOrdersAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // =====================================
        // Search Customers
        // =====================================

        public async Task<IEnumerable<Customer>>
            SearchCustomersAsync(string keyword)
        {
            keyword = keyword.Trim().ToLower();

            return await _dbSet
                .AsNoTracking()
                .Where(c =>
                    c.FullName.ToLower().Contains(keyword) || c.Phone.Contains(keyword))
                .ToListAsync();
        }

        // =====================================
        // Exists Check
        // =====================================

        public async Task<bool>
            CustomerExistsAsync(string phone)
        {
            return await _dbSet
                .AnyAsync(c => c.Phone == phone);
        }
    }
}
