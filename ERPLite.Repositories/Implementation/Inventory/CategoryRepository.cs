using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Repositories.Implementation.Inventory
{
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        // =====================================
        // Active Categories
        // =====================================

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }

        // =====================================
        // Category With Products
        // =====================================

        public async Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // =====================================
        // Exists Check
        // =====================================

        public async Task<bool> CategoryExistsAsync(string name)
        {
            return await _dbSet
                .AnyAsync(c => EF.Functions.Like(c.Name, $"%{name}%"));
        }
    }
}
