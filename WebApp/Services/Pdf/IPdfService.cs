namespace WebApp.Services.Pdf
{
    public enum PdfTemplateType
    {
        Estimate,
        InvoiceInState,
        InvoiceOutOfState,
        RetailThermal
    }

    public interface IPdfService
    {
        // Generate PDF bytes in memory for preview or printing
        System.Threading.Tasks.Task<byte[]> GeneratePdfAsync(Models.Pdf.PdfBillModel model, PdfTemplateType template);

        // Optionally print PDF bytes to default printer (best-effort)
        System.Threading.Tasks.Task<bool> PrintPdfAsync(byte[] pdfBytes, string? printerName = null);
    }
}
