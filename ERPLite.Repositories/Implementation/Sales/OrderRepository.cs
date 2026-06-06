using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Sales;
using ERPLite.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Sales
{
    public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

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

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }


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


        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbSet
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
        }

        public async Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
        }

        public async Task<bool> HasPaymentsAsync(int orderId)
        {
            return await _context.Payments
                .AnyAsync(p => p.OrderId == orderId);
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> GetPaidOrdersCountAsync()
        {
            return await _dbSet
                .CountAsync(x => x.PaymentStatus == OrderPaymentStatus.Paid);
        }

        public async Task<int> GetUnpaidOrdersCountAsync()
        {
            return await _dbSet
                .CountAsync(x => x.PaymentStatus != OrderPaymentStatus.Paid);
        }

        public async Task<decimal> GetUnpaidRevenueAsync()
        {
            return await _dbSet
                .Where(o => o.PaymentStatus == OrderPaymentStatus.Unpaid)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
        }
    }
}
