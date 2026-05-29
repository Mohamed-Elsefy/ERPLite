using ERPLite.Data.Entities.Identity;
using ERPLite.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;


namespace ERPLite.Data.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { Roles.Admin, Roles.Manager, Roles.Employee };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }

            await CreateUserAsync(userManager, "System Admin", "admin@erplite.com", "Admin@123", Roles.Admin);
            await CreateUserAsync(userManager, "Sales Manager", "manager@erplite.com", "Manager@123", Roles.Manager);
            await CreateUserAsync(userManager, "Employee User", "employee@erplite.com", "Employee@123", Roles.Employee);
        }

        private static async Task CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string fullName,
            string email,
            string password,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null) return;

            var user = new ApplicationUser
            {
                FullName = fullName,
                Email = email,
                UserName = email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}