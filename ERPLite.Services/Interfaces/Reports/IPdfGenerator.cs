namespace ERPLite.Services.Interfaces.Reports
{
    public interface IPdfGenerator
    {
        byte[] GenerateReportPdf<T>(string title, T data);
    }
}
