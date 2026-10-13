using ERPLite.Data.Entities.Identity;
using ERPLite.Data.Entities.HR;
using ERPLite.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ERPLite.Data.Context;

namespace ERPLite.Data.Seeders
{
    /// <summary>
    /// Development-only seeder. Invoked from Program.cs exclusively when
    /// <c>ASPNETCORE_ENVIRONMENT == Development</c> - never in production.
    /// Credentials come from configuration ("SeedUsers" section) so no secrets live in code.
    /// </summary>
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            string[] roles = { Roles.Admin, Roles.Manager, Roles.Employee };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }

            var defaultDepartment = await context.Set<Department>().FirstOrDefaultAsync();
            if (defaultDepartment == null)
            {
                defaultDepartment = new Department { Name = "General Administration" };
                await context.Set<Department>().AddAsync(defaultDepartment);
                await context.SaveChangesAsync();
            }

            var seedUsers = new[]
            {
                GetSeedUser(configuration, "Admin", "System Admin", "admin@erplite.com", "Admin@123", Roles.Admin),
                GetSeedUser(configuration, "Manager", "Sales Manager", "manager@erplite.com", "Manager@123", Roles.Manager),
                GetSeedUser(configuration, "Employee", "Employee User", "employee@erplite.com", "Employee@123", Roles.Employee),
            };

            foreach (var seedUser in seedUsers)
            {
                await CreateUserWithEmployeeAsync(
                    userManager, context,
                    seedUser.FullName, seedUser.Email, seedUser.Password, seedUser.Role,
                    defaultDepartment.Id);
            }
        }

        private static (string FullName, string Email, string Password, string Role) GetSeedUser(
            IConfiguration configuration, string key, string defaultFullName, string defaultEmail, string defaultPassword, string role)
        {
            var section = configuration.GetSection($"SeedUsers:{key}");
            return (
                section["FullName"] ?? defaultFullName,
                section["Email"] ?? defaultEmail,
                section["Password"] ?? defaultPassword,
                role);
        }

        private static async Task CreateUserWithEmployeeAsync(
            UserManager<ApplicationUser> userManager,
            AppDbContext context,
            string fullName,
            string email,
            string password,
            string role,
            int departmentId)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null) return;

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var employee = new Employee
                {
                    FullName = fullName,
                    Email = email,
                    HireDate = DateTime.Today,
                    Salary = 0,
                    DepartmentId = departmentId
                };

                await context.Set<Employee>().AddAsync(employee);
                await context.SaveChangesAsync();

                var user = new ApplicationUser
                {
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    CreatedAt = DateTime.UtcNow,
                    EmployeeId = employee.Id
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);

                    employee.UserId = user.Id;
                    context.Set<Employee>().Update(employee);
                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
