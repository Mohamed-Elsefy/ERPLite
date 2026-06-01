using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Sales;
using ERPLite.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Sales
{
    public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<IEnumerable<Payment>> GetPaymentsByOrderAsync(int orderId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Order)
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }


        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }


        public async Task<decimal> GetTotalPaidAmountAsync(int orderId)
        {
            return await _dbSet
                .Where(x => x.OrderId == orderId)
                .SumAsync(x => (decimal?)x.Amount) ?? 0;
        }


        public async Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<decimal> GetGlobalTotalPaidAmountAsync()
        {
            return await _dbSet.SumAsync(p => (decimal?)p.Amount) ?? 0;
        }
    }
}
