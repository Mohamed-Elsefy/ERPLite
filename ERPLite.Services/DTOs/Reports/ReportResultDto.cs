namespace ERPLite.Services.DTOs.Reports
{
    public class ReportResultDto<T>
    {
        public string ReportName { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; }

        public T Data { get; set; } = default!;
    }
}
