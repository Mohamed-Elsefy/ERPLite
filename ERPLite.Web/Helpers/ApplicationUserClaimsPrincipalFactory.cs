using ERPLite.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ERPLite.Web.Helpers
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (user.EmployeeId.HasValue)
            {
                identity.AddClaim(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));

                var userWithEmployee = await UserManager.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (userWithEmployee?.Employee != null)
                {
                    identity.AddClaim(new Claim("DepartmentId", userWithEmployee.Employee.DepartmentId.ToString()));
                }
            }

            return identity;
        }
    }
}