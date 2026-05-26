using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Sales;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Sales
{
    public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        // =====================================
        // Order With Full Details
        // =====================================

        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.CreatedByUser)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // =====================================
        // Recent Orders
        // =====================================

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }

        // =====================================
        // Orders By Customer
        // =====================================

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Payments)
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // =====================================
        // Orders By Date Range
        // =====================================

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Customer)
                .Where(o =>
                    o.OrderDate >= startDate &&
                    o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // =====================================
        // Total Revenue
        // =====================================

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbSet
            .Select(o => (decimal?)o.TotalPrice)
            .SumAsync() ?? 0;
        }

        // =====================================
        // Revenue By Date Range
        // =====================================

        public async Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
            .Where(o =>
                o.OrderDate >= startDate &&
                o.OrderDate <= endDate)
            .Select(o => (decimal?)o.TotalPrice)
            .SumAsync() ?? 0;
        }
    }
}
