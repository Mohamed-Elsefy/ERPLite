using System;

namespace ERPLite.Web.ViewModels.Reports
{
    public class ReportFilterViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ReportType { get; set; }
    }
}
