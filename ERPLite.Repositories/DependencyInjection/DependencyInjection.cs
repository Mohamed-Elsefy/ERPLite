using ERPLite.Repositories.Implementation.Common;
using ERPLite.Repositories.Implementation.HR;
using ERPLite.Repositories.Implementation.Inventory;
using ERPLite.Repositories.Implementation.Sales;
using ERPLite.Repositories.Interfaces.Common;
using ERPLite.Repositories.Interfaces.HR;
using ERPLite.Repositories.Interfaces.Inventory;
using ERPLite.Repositories.Interfaces.Sales;
using Microsoft.Extensions.DependencyInjection;

namespace ERPLite.Repositories.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IAttendanceRepository, AttendanceRepository>();

            return services;
        }
    }
}
