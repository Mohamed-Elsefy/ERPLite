using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Inventory
{
    public class SupplierRepository : GenericRepository<Supplier, int>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context)
        {
        }

        // =====================================
        // Active Suppliers
        // =====================================

        public async Task<IEnumerable<Supplier>>  GetActiveSuppliersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================
        // Supplier With Products
        // =====================================

        public async Task<Supplier?> GetSupplierWithProductsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // =====================================
        // Search Suppliers
        // =====================================

        public async Task<IEnumerable<Supplier>> SearchSuppliersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return await GetAllAsync();

            keyword = keyword.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(s => EF.Functions.Like(s.Name, $"%{keyword}%")
                         || EF.Functions.Like(s.Phone, $"%{keyword}%"))
                .ToListAsync();
        }

        // =====================================
        // Exists Check
        // =====================================

        public async Task<bool> SupplierExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            name = name.Trim();

            return await _dbSet.AnyAsync(s => EF.Functions.Like(s.Name, name));
        }
    }
}
