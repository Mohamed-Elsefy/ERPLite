using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Services.DTOs.Dashboard;
using ERPLite.Services.Helpers;
using ERPLite.Services.Interfaces.Dashboard;
using ERPLite.Services.Interfaces.Infrastructure; 
using ERPLite.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Services.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DashboardService(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ServiceResult<DashboardStatisticsDto>> GetStatisticsAsync()
        {
            var today = _dateTimeProvider.Now.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var totalEmployees = await _unitOfWork.Employees.GetActiveCountAsync();
            var totalDepartments = await _unitOfWork.Departments.GetCountAsync();
            var totalProducts = await _unitOfWork.Products.GetTotalProductsCountAsync();
            var totalCustomers = await _unitOfWork.Customers.GetCountAsync();
            var totalOrders = await _unitOfWork.Orders.GetCountAsync();
            var totalRevenue = await _unitOfWork.Orders.GetTotalRevenueAsync();
            var lowStockProductsList = await _unitOfWork.Products.GetLowStockProductsAsync();
            var monthlyRevenue = await _unitOfWork.Orders.GetRevenueByDateRangeAsync(startOfMonth, endOfMonth);
            var newOrdersThisMonth = await _unitOfWork.Orders.CountAsync(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth);
            var outOfStockProducts = await _unitOfWork.Products.GetOutOfStockCountAsync();
            var totalInventoryValue = await _unitOfWork.Products.GetInventoryValueAsync();
            var activeCustomers = await _unitOfWork.Customers.GetCountAsync();
            var paidOrders = await _unitOfWork.Orders.GetPaidOrdersCountAsync();
            var unpaidOrders = await _unitOfWork.Orders.GetUnpaidOrdersCountAsync();
            var outstandingBalance = await _unitOfWork.Orders.GetUnpaidRevenueAsync();
            var totalPaidRevenue = await _unitOfWork.Payments.GetGlobalTotalPaidAmountAsync();
            var partialPaidOrders = await _unitOfWork.Orders.CountAsync(o => o.PaymentStatus == OrderPaymentStatus.PartiallyPaid);

            int newCustomersThisMonth = 0;
            try
            {
                newCustomersThisMonth = await _unitOfWork.Customers.CountAsync(c => EF.Property<DateTime>(c, "CreatedAt") >= startOfMonth && EF.Property<DateTime>(c, "CreatedAt") <= endOfMonth);
            }
            catch
            {
                newCustomersThisMonth = 0;
            }

            var dto = new DashboardStatisticsDto
            {
                TotalEmployees = totalEmployees,
                TotalDepartments = totalDepartments,
                TotalProducts = totalProducts,
                TotalCustomers = totalCustomers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                LowStockProducts = lowStockProductsList?.Count() ?? 0,
                MonthlyRevenue = monthlyRevenue,
                NewOrdersThisMonth = newOrdersThisMonth,
                OutOfStockProducts = outOfStockProducts,
                NewCustomersThisMonth = newCustomersThisMonth,
                TotalInventoryValue = totalInventoryValue,
                ActiveCustomers = activeCustomers,
                PaidOrders = paidOrders,
                UnpaidOrders = unpaidOrders,
                OutstandingBalance = outstandingBalance,
                TotalPaidRevenue = totalPaidRevenue,
                PartialPaidOrders = partialPaidOrders
            };

            return ServiceResult<DashboardStatisticsDto>.Successful(dto);
        }

        public async Task<ServiceResult<AttendanceDashboardDataDto>> GetAttendanceDashboardRecordsAsync(int? filterDepartmentId)
        {
            try
            {
                var totalEmployees = await _unitOfWork.Employees.GetActiveCountByDepartmentAsync(filterDepartmentId);
                var todayAttendances = await _unitOfWork.Attendances.GetTodayAttendanceManagementAsync(filterDepartmentId);
                var attendanceList = todayAttendances.ToList();

                var presentCount = attendanceList.Count;

                var lateCount = attendanceList.Count(a => a.Status == AttendanceStatus.Late);
                var absentCount = totalEmployees - presentCount;

                if (absentCount < 0) absentCount = 0;

                var recordsDto = attendanceList.Select(a => new AttendanceManagementDto
                {
                    AttendanceId = a.Id,
                    EmployeeName = a.Employee?.FullName ?? "Unknown Employee",
                    DepartmentName = a.Employee?.Department?.Name ?? "No Department",
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status.ToString()
                }).ToList();

                var dashboardData = new AttendanceDashboardDataDto
                {
                    TotalEmployees = totalEmployees,
                    PresentCount = presentCount,
                    AbsentCount = absentCount,
                    LateCount = lateCount,
                    Records = recordsDto
                };

                return ServiceResult<AttendanceDashboardDataDto>.Successful(dashboardData);
            }
            catch (Exception ex)
            {
                return ServiceResult<AttendanceDashboardDataDto>.Failed($"Error loading attendance analytics: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<AttendanceManagementDto>>> GetEmployeeHistoryAsync(int employeeId)
        {
            try
            {
                var history = await _unitOfWork.Attendances.GetEmployeeAttendanceAsync(employeeId);

                var dto = history.Select(a => new AttendanceManagementDto
                {
                    AttendanceId = a.Id,
                    EmployeeName = a.Employee?.FullName ?? "Unknown",
                    DepartmentName = a.Employee?.Department?.Name ?? "No Department",
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status.ToString()
                }).ToList();

                return ServiceResult<IEnumerable<AttendanceManagementDto>>.Successful(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<AttendanceManagementDto>>.Failed($"Failed to load employee history: {ex.Message}");
            }
        }
    }
}