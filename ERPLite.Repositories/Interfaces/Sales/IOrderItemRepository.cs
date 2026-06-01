using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.Sales
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem, int>
    {
    }
}
