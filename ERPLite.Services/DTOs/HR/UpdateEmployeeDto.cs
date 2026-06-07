namespace ERPLite.Services.DTOs.HR
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public int DepartmentId { get; set; }

        public string? UserId { get; set; }
    }
}
