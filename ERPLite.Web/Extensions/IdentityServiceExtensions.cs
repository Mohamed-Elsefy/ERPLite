using ERPLite.Data.Context;
using ERPLite.Data.Entities.Identity;
using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Services.Services.Infrastructure;
using ERPLite.Shared.Constants;
using ERPLite.Web.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace ERPLite.Web.Extensions
{
    public static class IdentityServiceExtensions
    {
        // NOTE: ERPLite.Data types are used here intentionally - ASP.NET Core Identity
        // must be wired with the concrete ApplicationUser/ApplicationRole entity types.
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IWebHostEnvironment environment)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "ERPLite.Auth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/Auth/Login";
                options.LogoutPath = "/Auth/Logout";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                // SecurePolicy.Always breaks http://localhost in Development (cookie never sent),
                // so relax it locally and enforce HTTPS-only cookies in higher environments.
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(Roles.Admin));

                options.AddPolicy("EmployeeOnly", policy =>
                    policy.RequireRole(Roles.Employee));

                options.AddPolicy("ManagerOnly", policy =>
                    policy.RequireRole(Roles.Manager));

                options.AddPolicy("RequireManagerOrAdmin", policy =>
                    policy.RequireRole(Roles.Admin, Roles.Manager));

                options.AddPolicy("AuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser());
            });

            return services;
        }
    }
}