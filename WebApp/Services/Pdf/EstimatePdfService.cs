using QuestPDF.Fluent;
using QuestPDF.Helpers;
using WebApp.Models.Pdf;

namespace WebApp.Services.Pdf
{
    // Dedicated Estimate PDF generator implementing the Estimate layout
    public class EstimatePdfService : IPdfService
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        public EstimatePdfService(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        public Task<bool> PrintPdfAsync(byte[] pdfBytes, string? printerName = null)
        {
            // No-op printing for now
            return Task.FromResult(false);
        }

        public Task<byte[]> GeneratePdfAsync(PdfBillModel model, PdfTemplateType template)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        // Left logo and licence
                        row.ConstantColumn(100).Column(col =>
                        {
                            var logoPath = Path.Combine(_env.WebRootPath ?? string.Empty, "images", "Ganesha.png");
                            if (File.Exists(logoPath))
                            {
                                var safe = PdfServiceShared.PrepareImage(logoPath, 60, 60);
                                col.Item().Width(60).Height(60).Image(safe);
                            }
                        });

                        // Center title and company
                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().Text("ESTIMATE").FontSize(12).AlignCenter().Bold();
                            col.Item().Text(model.CompanyName).FontSize(16).Bold().AlignCenter();
                            col.Item().Text(model.CompanyAddress.Replace("\n", "\n")).FontSize(9).AlignCenter();
                        });

                        // Right: BillNo and Date
                        row.ConstantColumn(140).Column(col =>
                        {
                            col.Item().Text("Bill No : " + model.BillNo).AlignRight();
                            col.Item().Text("Date : " + model.BillDate.ToString("dd MMM yyyy")).AlignRight();
                        });
                    });

                    page.Content().PaddingTop(8).Column(col =>
                    {
                        // Customer details: name & phone on one row, address on the next
                        col.Item().Column(cu =>
                        {
                            cu.Item().Row(r =>
                            {
                                r.ConstantColumn(40).Column(left =>
                                {
                                    left.Item().Text("Name").Bold().FontSize(9);
                                });
                                r.RelativeColumn().Column(left =>
                                {
                                    left.Item().Text(model.CustomerName).FontSize(9);
                                });

                                r.ConstantColumn(50).Column(left =>
                                {
                                    left.Item().Text("Phone No: ").Bold().FontSize(9);
                                });
                                r.ConstantColumn(100).Column(right =>
                                {
                                    if (!string.IsNullOrWhiteSpace(model.CustomerMobile))
                                        right.Item().Text(model.CustomerMobile).AlignRight().FontSize(9);
                                });
                            });

                            // Address on a separate row beneath
                            cu.Item().PaddingTop(6).Row(r2 =>
                            {
                                r2.ConstantItem(40).Column(addr =>
                                {
                                    addr.Item().Text("Address").Bold().FontSize(9);
                                });
                                r2.RelativeColumn().Column(addr =>
                                {
                                    addr.Item().Text(model.CustomerAddress).FontSize(9);
                                });
                            });
                        });

                        // Items table header
                        col.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(36); // SL.No
                                columns.ConstantColumn(40); // Comp
                                columns.RelativeColumn(); // Particulars
                                columns.ConstantColumn(60); // Quantity
                                columns.ConstantColumn(40); // Per
                                columns.ConstantColumn(60); // Rate
                                columns.ConstantColumn(80); // Amount
                            });

                            // Header row
                            table.Header(header =>
                            {
                                header.Cell().Element(PdfServiceShared.TableCellStyle).Text("SL.No");
                                header.Cell().Element(PdfServiceShared.TableCellStyle).Text("Comp");
                                header.Cell().Element(PdfServiceShared.TableCellStyle).Text("Particulars");
                                header.Cell().Element(PdfServiceShared.TableCellStyle).AlignRight().Text("Quantity");
                                header.Cell().Element(PdfServiceShared.TableCellStyle).Text("Per");
                                header.Cell().Element(PdfServiceShared.TableCellStyle).AlignRight().Text("Rate");
                                header.Cell().Element(PdfServiceShared.TableCellStyle).AlignRight().Text("Amount");
                            });

                            // Data rows
                            decimal grandTotal = 0m;
                            foreach (var it in model.Items)
                            {
                                var lineAmount = it.Qty * it.Rate; // For estimates total = quantity * item price (no taxes)
                                grandTotal += lineAmount;
                                table.Cell().Element(PdfServiceShared.TableCellStyle).Text(it.SlNo.ToString());
                                table.Cell().Element(PdfServiceShared.TableCellStyle).Text(it.CompanyName);
                                table.Cell().Element(PdfServiceShared.TableCellStyle).Text(it.ItemName);
                                var quantityText = string.IsNullOrWhiteSpace(it.OriginalEntry)
                                    ? it.NormalizedQuantity.ToString("0.##")
                                    : $"{it.OriginalEntry} -> {it.NormalizedQuantity:0.##}";
                                table.Cell().Element(PdfServiceShared.TableCellStyle).AlignRight().Text(quantityText);
                                table.Cell().Element(PdfServiceShared.TableCellStyle).Text(it.TypeName ?? string.Empty);
                                table.Cell().Element(PdfServiceShared.TableCellStyle).AlignRight().Text(it.Rate.ToString("0.00"));
                                table.Cell().Element(PdfServiceShared.TableCellStyle).AlignRight().Text(lineAmount.ToString("0.00"));
                            }

                            // Totals row (compute from quantity * rate for each item)
                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(6).Element(f => f.AlignRight().Text("Total").FontSize(10).Bold());
                                footer.Cell().Element(f => f.AlignRight().Text(grandTotal.ToString("0.00")).FontSize(10).Bold());
                            });

                            // Use shared cell style to ensure full border
                            // (Kept local function removed - use PdfServiceShared.TableCellStyle)
                        });
                    });

                    page.Footer().AlignCenter().Text($"Bill No : {model.BillNo}    Page 1 of 1").FontSize(8);
                });
            });

            var bytes = doc.GeneratePdf();
            return Task.FromResult(bytes);
        }
    }
}
