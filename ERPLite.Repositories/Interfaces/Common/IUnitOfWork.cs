using ERPLite.Repositories.Interfaces.HR;
using ERPLite.Repositories.Interfaces.Inventory;
using ERPLite.Repositories.Interfaces.Sales;
using ERPLite.Repositories.Interfaces.System;
using Microsoft.EntityFrameworkCore.Storage;

namespace ERPLite.Repositories.Interfaces.Common
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IProductRepository Products { get; }

        ICategoryRepository Categories { get; }

        ISupplierRepository Suppliers { get; }

        ICustomerRepository Customers { get; }

        IOrderRepository Orders { get; }

        IPaymentRepository Payments { get; }

        IAttendanceRepository Attendances { get; }

        IEmployeeRepository Employees { get; }

        IDepartmentRepository Departments { get; }

        IOrderItemRepository OrderItems { get; }

        IActivityLogRepository ActivityLogs { get; }

        INotificationRepository Notifications { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists pending changes and commits the current transaction.
        /// Call this once per transaction instead of calling
        /// <see cref="SaveChangesAsync"/> beforehand - the extra save is redundant.
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
