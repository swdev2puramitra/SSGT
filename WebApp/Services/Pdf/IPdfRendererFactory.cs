using WebApp.Models.Pdf;

namespace WebApp.Services.Pdf
{
    public interface IPdfRendererFactory
    {
        // Resolve renderer for the requested template type and generate PDF
        Task<byte[]> GeneratePdfAsync(PdfBillModel model, PdfTemplateType template);
    }
}
