using ERPLite.Data.Entities.Identity;

namespace ERPLite.Data.Entities.HR
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}