using ERPLite.Data.Context;
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

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<bool> CategoryExistsAsync(string name, int? excludedCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            var cleanName = name.Trim();

            return await _dbSet.AnyAsync(c =>
                c.Name == cleanName &&
                (!excludedCategoryId.HasValue || c.Id != excludedCategoryId.Value));
        }
    }
}
