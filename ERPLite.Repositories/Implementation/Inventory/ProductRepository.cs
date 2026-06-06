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

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();
        }


        public async Task<Product?> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p =>
                    p.QuantityInStock <= p.MinStockLevel)
                .ToListAsync();
        }


        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetAllAsync();

            var cleanKeyword = keyword.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(p => EF.Functions.Like(p.Name, $"%{cleanKeyword}%"))
                .ToListAsync();
        }


        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category) 
                .Include(p => p.Supplier) 
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludedProductId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var cleanName = name.Trim();

            return await _dbSet.AnyAsync(x =>
                x.Name == cleanName &&
                (!excludedProductId.HasValue || x.Id != excludedProductId.Value));
        }

        public async Task<bool> HasAnyOrdersAsync(int productId)
        {
            return await _context.OrderItems
                .AnyAsync(x => x.ProductId == productId);
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> GetOutOfStockCountAsync()
        {
            return await _dbSet.CountAsync(x => x.QuantityInStock <= 0);
        }

        public async Task<decimal> GetInventoryValueAsync()
        {
            return await _dbSet.SumAsync(x =>
                    x.Price * x.QuantityInStock);
        }

        public async Task<Product?> GetForOrderAsync(int id)
        {
            return await _dbSet
                .AsNoTracking() 
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
