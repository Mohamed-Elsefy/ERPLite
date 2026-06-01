using System.Threading.Tasks;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Dashboard;
using ERPLite.Services.Interfaces.Dashboard;

namespace ERPLite.Services.Services.Dashboard
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SalesAnalyticsDto> GetSalesAnalyticsAsync()
        {
            var totalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync();
            var totalPaidRevenue = await _unitOfWork.Payments.GetGlobalTotalPaidAmountAsync();
            var outstandingBalance = totalRevenue - totalPaidRevenue;
            var paidOrders = await _unitOfWork.Orders.GetPaidOrdersCountAsync();
            var unpaidOrders = await _unitOfWork.Orders.GetUnpaidOrdersCountAsync();

            return new SalesAnalyticsDto
            {
                TotalRevenue = totalRevenue,
                TotalPaidRevenue = totalPaidRevenue,
                OutstandingBalance = outstandingBalance,
                PaidOrders = paidOrders,
                UnpaidOrders = unpaidOrders,
                TotalOrders = paidOrders + unpaidOrders
            };
        }
    }
}