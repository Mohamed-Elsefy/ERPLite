using ERPLite.Data.Entities.Inventory;
using ERPLite.Repositories.Extensions;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.Inventory
{
    public interface IProductRepository : IGenericRepository<Product, int>
    {
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();

        Task<PaginationResult<Product>> GetPagedWithDetailsAsync(string? search, int pageNumber, int pageSize);

        Task<Product?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Product>> GetLowStockProductsAsync();

        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);

        Task<bool> ExistsByNameAsync(string name, int? excludedProductId = null);

        Task<bool> HasAnyOrdersAsync(int productId);

        Task<int> GetTotalProductsCountAsync();

        Task<int> GetOutOfStockCountAsync();

        Task<int> GetLowStockCountAsync();

        Task<decimal> GetInventoryValueAsync();

        Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> ids);
    }
}
