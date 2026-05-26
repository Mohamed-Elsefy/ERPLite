using ERPLite.Repositories.Interfaces.HR;
using ERPLite.Repositories.Interfaces.Inventory;
using ERPLite.Repositories.Interfaces.Sales;
using Microsoft.EntityFrameworkCore.Storage;

namespace ERPLite.Repositories.Interfaces.Common
{
    public interface IUnitOfWork : IDisposable
    {

        // =================================
        // Registe Repositories
        // =================================
        IProductRepository Products { get; }

        ICategoryRepository Categories { get; }

        ISupplierRepository Suppliers { get; }

        ICustomerRepository Customers { get; }

        IOrderRepository Orders { get; }

        IPaymentRepository Payments { get; }

        IAttendanceRepository Attendances { get; }


        // =================================
        // Save Changes
        // =================================

        Task<int> SaveChangesAsync();

        // =================================
        // Transactions
        // =================================

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
