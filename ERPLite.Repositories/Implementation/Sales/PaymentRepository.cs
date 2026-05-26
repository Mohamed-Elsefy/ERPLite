using ERPLite.Data.Entities.Sales;
using ERPLite.Data.Enums;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Sales;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Sales
{
    public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        // =====================================
        // Payments By Order
        // =====================================

        public async Task<IEnumerable<Payment>> GetPaymentsByOrderAsync(int orderId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Order)
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        // =====================================
        // Payments By Status
        // =====================================

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        // =====================================
        // Total Paid Amount
        // =====================================

        public async Task<decimal> GetTotalPaidAmountAsync(int orderId)
        {
            return await _dbSet
                .Where(p => p.OrderId == orderId)
                .Select(p => (decimal?)p.Amount)
                .SumAsync() ?? 0;
        }

        // =====================================
        // Recent Payments
        // =====================================

        public async Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
