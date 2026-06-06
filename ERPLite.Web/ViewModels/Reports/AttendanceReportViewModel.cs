using System;
using System.Collections.Generic;

namespace ERPLite.Web.ViewModels.Reports
{
    public class AttendanceReportViewModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int TotalRecords { get; set; }
        public int PresentCount { get; set; }
        public int LateCount { get; set; }
        public List<object>? AttendanceRecords { get; set; }
    }
}
