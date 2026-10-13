namespace ERPLite.Shared.Constants
{
    /// <summary>
    /// Canonical audit-trail action names. Use these in <c>LogAsync(action: ...)</c>
    /// instead of inline strings so filtering/reporting stays consistent.
    /// </summary>
    public static class AuditActions
    {
        public const string Create = "Create";

        public const string Update = "Update";

        public const string Delete = "Delete";

        public const string Login = "Login";

        public const string Logout = "Logout";

        public const string ChangePassword = "ChangePassword";

        public const string Lock = "Lock";

        public const string Unlock = "Unlock";

        public const string GrantAccess = "GrantAccess";

        public const string UpdateRole = "UpdateRole";

        public const string DeleteUserIdentity = "DeleteUserIdentity";

        public const string AssignRole = "AssignRole";

        public const string RemoveRole = "RemoveRole";

        public const string CheckIn = "CheckIn";

        public const string CheckOut = "CheckOut";
    }
}
