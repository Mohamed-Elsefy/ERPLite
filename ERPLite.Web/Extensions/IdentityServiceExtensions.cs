using ERPLite.Data.Entities.Identity;
using ERPLite.Shared.Constants;
using ERPLite.Web.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace ERPLite.Web.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    // Password Settings
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;

                    // User Settings
                    options.User.RequireUniqueEmail = true;

                    // Lockout Settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    // try 5 times, if fails lockout for 15 minutes
                    options.Lockout.MaxFailedAccessAttempts = 5;

                    // aplay settings for new users
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

                options.SessionStore = null;

                options.ExpireTimeSpan = TimeSpan.FromDays(7);

                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "AdminOnly",
                    policy => policy.RequireRole(Roles.Admin));

                options.AddPolicy(
                    "ManagerOnly",
                    policy => policy.RequireRole(
                        Roles.Admin,
                        Roles.Manager));

                options.AddPolicy(
                    "EmployeeOnly",
                    policy => policy.RequireRole(
                        Roles.Employee));

                options.AddPolicy(
                    "AllUsers",
                    policy => policy.RequireRole(
                        Roles.Admin,
                        Roles.Manager,
                        Roles.Employee));
            });

            return services;
        }
    }
}
