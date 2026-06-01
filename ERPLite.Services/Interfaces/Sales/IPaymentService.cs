using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Sales
{
    public interface IPaymentService
    {
        Task<ServiceResult<IEnumerable<PaymentDto>>> GetPaymentsByOrderAsync(int orderId);

        Task<ServiceResult<OrderFinancialDto>> GetOrderFinancialSummaryAsync(int orderId);

        Task<ServiceResult<int>> CreatePaymentAsync(CreatePaymentDto dto, string currentUserId);
    }
}