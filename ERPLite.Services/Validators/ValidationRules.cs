using ERPLite.Shared.Constants;

namespace ERPLite.Services.Validators
{
    /// <summary>
    /// Rules shared by multiple validators so messages stay identical everywhere.
    /// </summary>
    public static class ValidationRules
    {
        public static bool BeValidRole(string? role)
        {
            return role == Roles.Admin || role == Roles.Manager || role == Roles.Employee;
        }

        public const string InvalidRoleMessage = "Role must be Admin, Manager or Employee.";
    }
}
