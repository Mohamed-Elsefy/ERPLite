using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.Sales
{
    public interface IOrderRepository : IGenericRepository<Order, int>
    {
        Task<Order?> GetOrderWithDetailsAsync(int id);

        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);

        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);

        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<decimal> GetTotalRevenueAsync();

        Task<int> GetPaidOrdersCountAsync();

        Task<int> GetUnpaidOrdersCountAsync();

        Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<bool> HasPaymentsAsync(int orderId);

        Task<int> GetCountAsync();
    }
}
