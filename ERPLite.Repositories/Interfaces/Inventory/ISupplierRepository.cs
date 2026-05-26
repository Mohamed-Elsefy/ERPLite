using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.Inventory
{
    public interface ISupplierRepository : IGenericRepository<Supplier, int>
    {
        Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();

        Task<Supplier?> GetSupplierWithProductsAsync(int id);

        Task<IEnumerable<Supplier>> SearchSuppliersAsync(string keyword);

        Task<bool> SupplierExistsAsync(string name);
    }
}
