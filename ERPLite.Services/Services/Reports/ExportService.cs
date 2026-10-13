using ERPLite.Services.Interfaces.Reports;

namespace ERPLite.Services.Services.Reports
{
    public class ExportService : IExportService
    {
        private readonly IPdfGenerator _pdfGenerator;

        public ExportService(IPdfGenerator pdfGenerator)
        {
            _pdfGenerator = pdfGenerator;
        }

        public byte[] ExportToPdf<T>(string title, T data)
        {
            return _pdfGenerator.GenerateReportPdf(title, data);
        }

        public Task<byte[]> ExportToPdfAsync<T>(string title, T data, CancellationToken cancellationToken = default)
        {
            return _pdfGenerator.GenerateReportPdfAsync(title, data, cancellationToken);
        }
    }
}
