using ERPLite.Services.DTOs.Inventory;
using ERPLite.Services.Helpers;

namespace ERPLite.Services.Interfaces.Inventory
{
    public interface IInventoryService
    {
        Task<ServiceResult> StockInAsync(
            int productId,
            int quantity,
            string? notes = null);

        Task<ServiceResult> StockOutAsync(
            int productId,
            int quantity,
            string? notes = null);

        Task<ServiceResult> AdjustStockAsync(
            int productId,
            int actualQuantity,
            string? notes = null);

        Task<ServiceResult<IEnumerable<StockMovementDto>>> GetHistoryAsync(int productId);
        Task<ServiceResult<IEnumerable<StockMovementDto>>> GetAllHistoryAsync();
    }
}
