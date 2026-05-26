using ERPLite.Services.Interfaces.Auth;
using ERPLite.Services.Services.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace ERPLite.Services.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
