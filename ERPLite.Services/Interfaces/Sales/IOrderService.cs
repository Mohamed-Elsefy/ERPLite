using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Sales
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderDto>> GetOrderDetailsAsync(int id);

        Task<ServiceResult<IEnumerable<OrderDto>>> GetRecentOrdersAsync();

        Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersByCustomerAsync(int customerId);

        Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<ServiceResult<decimal>> GetTotalRevenueAsync();

        Task<ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto, string currentUserId);
    }
}