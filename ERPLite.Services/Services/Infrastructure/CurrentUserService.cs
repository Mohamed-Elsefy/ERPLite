using ERPLite.Services.Interfaces.Infrastructure;
using ERPLite.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ERPLite.Services.Services.Infrastructure
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor _httpContextAccessor)
        {
            this._httpContextAccessor = _httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? UserId => User?.GetUserId();

        public string? Email => User?.GetUserEmail();

        public int? EmployeeId
        {
            get
            {
                var claim = User?.FindFirst("EmployeeId")?.Value;
                return int.TryParse(claim, out int id) ? id : null;
            }
        }

        public int? DepartmentId
        {
            get
            {
                var claim = User?.FindFirst("DepartmentId")?.Value;
                return int.TryParse(claim, out int id) ? id : null;
            }
        }

        public bool IsAdmin => User?.IsAdmin() ?? false;

        public bool IsManager => User?.IsManager() ?? false;

        public bool IsEmployee => User?.IsEmployee() ?? false;
    }
}
