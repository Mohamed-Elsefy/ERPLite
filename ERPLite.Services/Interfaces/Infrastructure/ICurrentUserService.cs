namespace ERPLite.Services.Interfaces.Infrastructure
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        int? EmployeeId { get; }
        int? DepartmentId { get; }
        bool IsAdmin { get; }
        bool IsManager { get; }
        bool IsEmployee { get; }
    }
}
