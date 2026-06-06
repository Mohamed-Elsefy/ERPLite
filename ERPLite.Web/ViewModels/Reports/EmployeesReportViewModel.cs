using System.Collections.Generic;

namespace ERPLite.Web.ViewModels.Reports
{
    public class EmployeesReportViewModel
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public decimal TotalSalaries { get; set; }
        public List<object>? Employees { get; set; }
    }
}
