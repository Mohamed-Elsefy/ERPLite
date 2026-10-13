namespace ERPLite.Services.Interfaces.Reports
{
    public interface IPdfGenerator
    {
        byte[] GenerateReportPdf<T>(string title, T data);

        /// <summary>
        /// Offloads CPU-bound QuestPDF rendering to the thread pool so the
        /// request thread is not blocked during large report generation.
        /// </summary>
        Task<byte[]> GenerateReportPdfAsync<T>(string title, T data, CancellationToken cancellationToken = default);
    }
}
