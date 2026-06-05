namespace ERPLite.Web.ViewModels.Reports
{
    public class FinancialReportViewModel
    {
        public decimal Revenue { get; set; }
        public decimal Payments { get; set; }
        public decimal OutstandingBalance { get; set; }
        public int PaidOrders { get; set; }
        public int UnpaidOrders { get; set; }
    }
}
