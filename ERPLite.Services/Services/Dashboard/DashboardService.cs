using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Dashboard;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Dashboard;

namespace ERPLite.Services.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<DashboardStatisticsDto>> GetStatisticsAsync()
        {
            var totalEmployees = await _unitOfWork.Employees.GetActiveCountAsync();
            var totalDepartments = await _unitOfWork.Departments.GetCountAsync();
            var totalProducts = await _unitOfWork.Products.GetTotalProductsCountAsync();
            var totalCustomers = await _unitOfWork.Customers.GetCountAsync();
            var totalOrders = await _unitOfWork.Orders.GetCountAsync();
            var totalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync();
            var lowStockProductsList = await _unitOfWork.Products.GetLowStockProductsAsync();

            var dto = new DashboardStatisticsDto
            {
                TotalEmployees = totalEmployees,
                TotalDepartments = totalDepartments,
                TotalProducts = totalProducts,
                TotalCustomers = totalCustomers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                LowStockProducts = lowStockProductsList?.Count() ?? 0
            };

            return ServiceResult<DashboardStatisticsDto>.Successful(dto);
        }
    }
}