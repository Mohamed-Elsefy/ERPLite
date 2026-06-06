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

        Task<bool> ExistsByNameAsync(string name, int? excludedProductId = null);

        Task<bool> HasAnyOrdersAsync(int productId);

        Task<int> GetTotalProductsCountAsync();

        Task<int> GetOutOfStockCountAsync();

        Task<decimal> GetInventoryValueAsync();

        Task<Product?> GetForOrderAsync(int id);

        Task<bool> ExistsBySkuAsync(string sku);

        Task<bool> ExistsBySkuAsync(
            string sku,
            int excludedId);

        Task<Product?> GetLastCreatedProductAsync();
    }
}
