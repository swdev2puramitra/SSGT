using WebApp.Models.Pdf;

namespace WebApp.Services.Pdf
{
    // Facade/router that exposes a single IPdfService and delegates to template-specific implementations.
    public class PdfServiceRouter : IPdfService
    {
        private readonly EstimatePdfService _estimateService;
        private readonly InvoiceInStatePdfService _invoiceInStateService;
        private readonly InvoiceOutOfStatePdfService _invoiceOutOfStateService;

        public PdfServiceRouter(EstimatePdfService estimateService, InvoiceInStatePdfService invoiceInStateService, InvoiceOutOfStatePdfService invoiceOutOfStateService)
        {
            _estimateService = estimateService ?? throw new ArgumentNullException(nameof(estimateService));
            _invoiceInStateService = invoiceInStateService ?? throw new ArgumentNullException(nameof(invoiceInStateService));
            _invoiceOutOfStateService = invoiceOutOfStateService ?? throw new ArgumentNullException(nameof(invoiceOutOfStateService));
        }

        public async Task<byte[]> GeneratePdfAsync(PdfBillModel model, PdfTemplateType template)
        {
            switch (template)
            {
                case PdfTemplateType.Estimate:
                    return await _estimateService.GeneratePdfAsync(model, template).ConfigureAwait(false);
                case PdfTemplateType.InvoiceInState:
                    return await _invoiceInStateService.GeneratePdfAsync(model, template).ConfigureAwait(false);
                case PdfTemplateType.InvoiceOutOfState:
                    // Delegate to the dedicated out-of-state renderer which uses IGST-only taxable summary
                    return await _invoiceOutOfStateService.GeneratePdfAsync(model, template).ConfigureAwait(false);
                case PdfTemplateType.RetailThermal:
                    // Retail thermal can be handled by a dedicated service in future; reuse invoice service for now
                    return await _invoiceInStateService.GeneratePdfAsync(model, template).ConfigureAwait(false);
                default:
                    return await _estimateService.GeneratePdfAsync(model, template).ConfigureAwait(false);
            }
        }

        public Task<bool> PrintPdfAsync(byte[] pdfBytes, string? printerName = null)
        {
            // Delegate printing to invoice service by default; services may implement printing or return false.
            return _invoiceInStateService.PrintPdfAsync(pdfBytes, printerName);
        }
    }
}
