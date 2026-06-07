using ERPLite.Data.Context;
using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Interfaces.Sales;

namespace ERPLite.Repositories.Implementation.Sales
{
    public class OrderItemRepository : GenericRepository<OrderItem, int>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
