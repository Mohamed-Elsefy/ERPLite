using ERPLite.Data.Context;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Repositories.Interfaces.HR;
using ERPLite.Repositories.Interfaces.Inventory;
using ERPLite.Repositories.Interfaces.Sales;
using ERPLite.Repositories.Interfaces.System;
using Microsoft.EntityFrameworkCore.Storage;


namespace ERPLite.Repositories.Implementation.Common
{
    public class UnitOfWork(
            AppDbContext context,
            IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            ISupplierRepository supplierRepo,
            ICustomerRepository customerRepo,
            IOrderRepository orderRepo,
            IPaymentRepository paymentRepo,
            IAttendanceRepository attendanceRepo,
            IEmployeeRepository employeeRepo,
            IDepartmentRepository departmentRepo,
            IOrderItemRepository orderItemRepo,
            IActivityLogRepository activityLogRepo,
            INotificationRepository notificationRepo
        ) : IUnitOfWork
    {
        private IDbContextTransaction? _currentTransaction;

        public IProductRepository Products { get; } = productRepo;
        public ICategoryRepository Categories { get; } = categoryRepo;
        public ISupplierRepository Suppliers { get; } = supplierRepo;
        public ICustomerRepository Customers { get; } = customerRepo;
        public IOrderRepository Orders { get; } = orderRepo;
        public IPaymentRepository Payments { get; } = paymentRepo;
        public IAttendanceRepository Attendances { get; } = attendanceRepo;
        public IEmployeeRepository Employees { get; } = employeeRepo;
        public IDepartmentRepository Departments { get; } = departmentRepo;
        public IOrderItemRepository OrderItems { get; } = orderItemRepo;
        public IActivityLogRepository ActivityLogs { get; } = activityLogRepo;
        public INotificationRepository Notifications { get; } = notificationRepo;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return _currentTransaction;

            _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await context.SaveChangesAsync(cancellationToken);

                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
