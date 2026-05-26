using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.Inventory
{
    public interface IProductRepository : IGenericRepository<Product, int>
    {
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();

        Task<Product?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Product>> GetLowStockProductsAsync();

        Task<IEnumerable<Product>> SearchProductsAsync(string keyword);

        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}
