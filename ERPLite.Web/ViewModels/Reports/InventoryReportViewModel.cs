using System.Collections.Generic;

namespace ERPLite.Web.ViewModels.Reports
{
    public class InventoryReportViewModel
    {
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal InventoryValue { get; set; }
        public List<object>? Products { get; set; }
    }
}
