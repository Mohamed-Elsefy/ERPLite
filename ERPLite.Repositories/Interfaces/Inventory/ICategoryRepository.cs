using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.Inventory
{
    public interface ICategoryRepository : IGenericRepository<Category, int>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();

        Task<Category?> GetCategoryWithProductsAsync(int id);

        Task<bool> CategoryExistsAsync(string name, int? excludedCategoryId = null);
    }
}
