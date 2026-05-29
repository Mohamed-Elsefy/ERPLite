using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Shared.Enums;

namespace ERPLite.Repositories.Interfaces.Sales
{
    public interface IPaymentRepository : IGenericRepository<Payment, int>
    {
        Task<IEnumerable<Payment>> GetPaymentsByOrderAsync(int orderId);

        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);

        Task<decimal> GetTotalPaidAmountAsync(int orderId);

        Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count);
    }
}
