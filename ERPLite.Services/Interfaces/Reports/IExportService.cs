namespace ERPLite.Services.Interfaces.Reports
{
    public interface IExportService
    {
        byte[] ExportToPdf<T>(string title, T data);

        Task<byte[]> ExportToPdfAsync<T>(string title, T data, CancellationToken cancellationToken = default);
    }
}
