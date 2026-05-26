using ERPLite.Data.Entities.Sales;
using ERPLite.Repositories.Interfaces.Common;

namespace ERPLite.Repositories.Interfaces.Sales
{
    public interface ICustomerRepository : IGenericRepository<Customer, int>
    {
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();

        Task<Customer?> GetCustomerWithOrdersAsync(int id);

        Task<IEnumerable<Customer>> SearchCustomersAsync(string keyword);

        Task<bool> CustomerExistsAsync(string phone);
    }
}
