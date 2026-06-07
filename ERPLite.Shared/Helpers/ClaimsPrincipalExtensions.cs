using ERPLite.Shared.Constants;
using System.Security.Claims;

namespace ERPLite.Shared.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetUserName(this ClaimsPrincipal user)
        {
            return user.Identity?.Name;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(Roles.Admin);
        }

        public static bool IsManager(this ClaimsPrincipal user)
        {
            return user.IsInRole(Roles.Manager);
        }

        public static bool IsEmployee(this ClaimsPrincipal user)
        {
            return user.IsInRole(Roles.Employee);
        }

        public static bool IsManagerOrAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(Roles.Admin) || user.IsInRole(Roles.Manager);
        }
    }
}