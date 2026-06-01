using AutoMapper;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.HR;
using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.DTOs.Reports;
using ERPLite.Services.DTOs.Sales;
using ERPLite.Services.Interfaces.Reports;
using ERPLite.Shared.Enums;

namespace ERPLite.Services.Reports.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReportResultDto<EmployeesReportDto>> GenerateEmployeesReportAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllWithDepartmentsAsync();
            var employeeList = employees.ToList();
            var activeEmployeesList = employeeList.Where(e => e.User == null || e.User.LockoutEnd == null || e.User.LockoutEnd < DateTimeOffset.UtcNow).ToList();

            var reportData = new EmployeesReportDto
            {
                TotalEmployees = employeeList.Count,
                ActiveEmployees = activeEmployeesList.Count,
                TotalSalaries = activeEmployeesList.Sum(e => e.Salary),
                Employees = _mapper.Map<List<EmployeeDto>>(employeeList)
            };

            return new ReportResultDto<EmployeesReportDto>
            {
                ReportName = "Corporate Human Resources & Payroll Summary",
                GeneratedAt = DateTime.UtcNow,
                Data = reportData
            };
        }

        public async Task<ReportResultDto<AttendanceReportDto>> GenerateAttendanceReportAsync(DateTime from, DateTime to)
        {
            var startDate = from.Date;
            var endDate = to.Date.AddDays(1).AddTicks(-1);

            var records = await _unitOfWork.Attendances.GetAttendanceByDateRangeAsync(startDate, endDate);
            var recordList = records.ToList();

            var reportData = new AttendanceReportDto
            {
                From = from,
                To = to,
                TotalRecords = recordList.Count,
                PresentCount = recordList.Count(r => r.Status == AttendanceStatus.Present),
                LateCount = recordList.Count(r => r.Status == AttendanceStatus.Late),
                Records = _mapper.Map<List<AttendanceDto>>(recordList)
            };

            return new ReportResultDto<AttendanceReportDto>
            {
                ReportName = $"Employee Attendance Sheet ({from:yyyy-MM-dd} to {to:yyyy-MM-dd})",
                GeneratedAt = DateTime.UtcNow,
                Data = reportData
            };
        }

        public async Task<ReportResultDto<InventoryReportDto>> GenerateInventoryReportAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var productList = products.ToList();
            var lowStockProducts = await _unitOfWork.Products.GetLowStockProductsAsync();

            var reportData = new InventoryReportDto
            {
                TotalProducts = productList.Count,
                LowStockProducts = lowStockProducts?.Count() ?? 0,
                OutOfStockProducts = productList.Count(p => p.QuantityInStock == 0),
                InventoryValue = productList.Sum(p => p.QuantityInStock * p.Price),
                Products = _mapper.Map<List<ProductDto>>(productList)
            };

            return new ReportResultDto<InventoryReportDto>
            {
                ReportName = "Inventory Asset Valuation & Stock Status",
                GeneratedAt = DateTime.UtcNow,
                Data = reportData
            };
        }

        public async Task<ReportResultDto<SalesReportDto>> GenerateSalesReportAsync(DateTime from, DateTime to)
        {
            var startDate = from.Date;
            var endDate = to.Date.AddDays(1).AddTicks(-1);

            var orders = await _unitOfWork.Orders.GetOrdersByDateRangeAsync(startDate, endDate);
            var orderList = orders.ToList();

            var totalSales = orderList.Sum(o => o.TotalPrice);
            var totalOrders = orderList.Count;

            var reportData = new SalesReportDto
            {
                From = from,
                To = to,
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                AverageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0,
                Orders = _mapper.Map<List<OrderDto>>(orderList)

            };

            return new ReportResultDto<SalesReportDto>
            {
                ReportName = $"Commercial Sales Performance Report ({from:yyyy-MM-dd} to {to:yyyy-MM-dd})",
                GeneratedAt = DateTime.UtcNow,
                Data = reportData
            };
        }

        public async Task<ReportResultDto<FinancialReportDto>> GenerateFinancialReportAsync()
        {
            var totalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync();
            var totalPaid = await _unitOfWork.Payments.GetGlobalTotalPaidAmountAsync();

            var paidOrders = await _unitOfWork.Orders.GetPaidOrdersCountAsync();
            var unpaidOrders = await _unitOfWork.Orders.GetUnpaidOrdersCountAsync();

            var reportData = new FinancialReportDto
            {
                TotalRevenue = totalRevenue,
                TotalPaid = totalPaid,
                OutstandingBalance = totalRevenue - totalPaid,
                PaidOrders = paidOrders,
                UnpaidOrders = unpaidOrders
            };

            return new ReportResultDto<FinancialReportDto>
            {
                ReportName = "Executive Financial Summary Statement",
                GeneratedAt = DateTime.UtcNow,
                Data = reportData
            };
        }
    }
}