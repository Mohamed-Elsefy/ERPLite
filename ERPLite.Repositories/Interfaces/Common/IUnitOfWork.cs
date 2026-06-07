using ERPLite.Repositories.Interfaces.HR;
using ERPLite.Repositories.Interfaces.Inventory;
using ERPLite.Repositories.Interfaces.Sales;
using ERPLite.Repositories.Interfaces.System;
using Microsoft.EntityFrameworkCore.Storage;

namespace ERPLite.Repositories.Interfaces.Common
{
    public interface IUnitOfWork : IDisposable
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


        Task<int> SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
