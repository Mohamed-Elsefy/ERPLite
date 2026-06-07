using ERPLite.Data.Entities.HR;
using Microsoft.AspNetCore.Identity;

namespace ERPLite.Data.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public int? EmployeeId { get; set; }

        public Employee? Employee { get; set; }
    }
}
