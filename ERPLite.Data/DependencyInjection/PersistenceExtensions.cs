using ERPLite.Data.Context;
using ERPLite.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERPLite.Data.DependencyInjection
{
    /// <summary>
    /// Composition root for the persistence layer. Web calls these extensions
    /// so it never touches <see cref="AppDbContext"/> namespaces directly.
    /// </summary>
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
        {
            return IdentitySeeder.SeedRolesAndUsersAsync(serviceProvider);
        }
    }
}
