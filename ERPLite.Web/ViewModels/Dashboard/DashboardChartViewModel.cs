using System.Collections.Generic;

namespace ERPLite.Web.ViewModels.Dashboard
{
    public class DashboardChartViewModel
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Values { get; set; } = new List<decimal>();
    }
}
