using System;
using System.Collections.Generic;

namespace ERPLite.Web.ViewModels.Reports
{
    public class SalesReportViewModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int OrdersCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<object>? Orders { get; set; }
    }
}
