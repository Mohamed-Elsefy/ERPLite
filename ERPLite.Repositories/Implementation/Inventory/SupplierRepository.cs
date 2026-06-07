using ERPLite.Data.Context;
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


        public async Task<IEnumerable<Supplier>>  GetActiveSuppliersAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Supplier?> GetSupplierWithProductsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);
        }


        public async Task<IEnumerable<Supplier>> SearchSuppliersAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return await GetAllAsync();

            var cleanKeyword = keyword.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(s => EF.Functions.Like(s.Name, $"%{cleanKeyword}%")
                         || EF.Functions.Like(s.Phone, $"%{cleanKeyword}%"))
                .ToListAsync();
        }


        public async Task<bool> SupplierExistsAsync(string name, int? excludedSupplierId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var cleanName = name.Trim();

            return await _dbSet.AnyAsync(s =>
                s.Name == cleanName &&
                (!excludedSupplierId.HasValue || s.Id != excludedSupplierId.Value));
        }

        public async Task<bool> HasProductsAsync(int supplierId)
        {
            return await _context.Products
                .AnyAsync(x => x.SupplierId == supplierId);
        }
    }
}
