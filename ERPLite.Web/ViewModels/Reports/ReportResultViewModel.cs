namespace ERPLite.Web.ViewModels.Reports
{
    public class ReportResultViewModel<T>
    {
        public string? ReportName { get; set; }
        public T? Data { get; set; }
    }
}
