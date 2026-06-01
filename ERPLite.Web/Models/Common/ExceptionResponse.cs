namespace ERPLite.Web.Models.Common
{
    public class ExceptionResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }
    }
}
