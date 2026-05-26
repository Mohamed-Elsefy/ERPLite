using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Inventory
{
    public class ProductRepository : GenericRepository<Product, int>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        // =====================================
        // Get All Products With Details
        // =====================================

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }

        // =====================================
        // Get Product By Id With Details
        // =====================================

        public async Task<Product?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // =====================================
        // Low Stock Products
        // =====================================

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p =>
                    p.QuantityInStock <= p.MinStockLevel)
                .ToListAsync();
        }

        // =====================================
        // Search
        // =====================================

        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return await GetAllAsync();

            keyword = keyword.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(p => EF.Functions.Like(p.Name, $"%{keyword}%"))
                .ToListAsync();
        }

        // =====================================
        // By Category
        // =====================================

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
