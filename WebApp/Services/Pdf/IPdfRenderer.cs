using WebApp.Models.Pdf;

namespace WebApp.Services.Pdf
{
    public interface IPdfRenderer
    {
        // Generate a PDF for the provided model. Renderer implementations decide template specifics.
        Task<byte[]> GeneratePdfAsync(PdfBillModel model);
    }
}
