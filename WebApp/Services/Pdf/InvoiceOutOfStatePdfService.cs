using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models.Pdf;

namespace WebApp.Services.Pdf
{
    // Dedicated renderer for out-of-state invoices. Reuses in-state layout but shows IGST-only tax summary.
    public class InvoiceOutOfStatePdfService : IPdfService
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        public InvoiceOutOfStatePdfService(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        private string PrepareImage(string sourcePath, int targetWidth = 800, int targetHeight = 800)
        {
            return PdfServiceShared.PrepareImage(sourcePath, targetWidth, targetHeight);
        }

        public async Task<byte[]> GeneratePdfAsync(PdfBillModel model, PdfTemplateType template)
        {
            // Build on the same overall layout as InvoiceInStatePdfService but adjust taxable summary to IGST-only
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(18);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header (same as in-state)
                    page.Header().Row(row =>
                    {
                        row.ConstantColumn(150).AlignMiddle().AlignLeft().PaddingLeft(2).Element(c =>
                        {
                            c.Column(col =>
                            {
                                if (!string.IsNullOrWhiteSpace(model.LicenceNo))
                                    col.Item().Text("Licence No.: " + model.LicenceNo).FontSize(8).AlignLeft();

                                var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? string.Empty, "images", "Ganesha.png");
                                if (System.IO.File.Exists(logoPath))
                                {
                                    var safeLogo = PrepareImage(logoPath, 76, 60);
                                    col.Item().Width(76).Height(60).AlignMiddle().AlignLeft().Image(safeLogo);
                                }
                            });
                        });

                        row.RelativeColumn().Column(col =>
                        {
                            col.Item().PaddingBottom(4).Element(c => c.Text("TAX INVOICE").FontSize(12).AlignCenter());
                            col.Item().PaddingTop(5).Text(model.CompanyName).Bold().FontSize(22).AlignCenter();
                            col.Item().PaddingTop(5).Text(model.CompanyAddress.Replace("\n", "\n")).FontSize(9f).AlignCenter();
                            col.Item().PaddingTop(5).Text("GSTIN: " + model.Gstin).FontSize(9f).AlignCenter();
                        });

                        row.ConstantItem(160).Column(col =>
                        {
                            col.Item().Text("Bill No: " + model.InvoiceNo).AlignRight();
                            col.Item().Text("Date: " + model.BillDate.ToString("dd MMM yyyy")).AlignRight();

                            var stampPath = System.IO.Path.Combine(_env.WebRootPath ?? string.Empty, "images", "stamp.png");
                            if (System.IO.File.Exists(stampPath))
                            {
                                var safeStamp = PrepareImage(stampPath, 76, 60);
                                col.Item().PaddingTop(6).AlignRight().Width(76).Height(60).Image(safeStamp);
                            }
                        });
                    });

                    page.Content().PaddingVertical(5).Column(col =>
                    {
                        // Customer and meta
                        col.Item().Row(r2 =>
                        {
                            r2.RelativeColumn().Column(cu =>
                            {
                                cu.Item().Text("Customer: " + model.CustomerName).FontSize(10).Bold();
                                cu.Item().Text(model.CustomerAddress).FontSize(9f);
                            });

                            r2.ConstantColumn(160).AlignRight().Text(x =>
                            {
                                if (!string.IsNullOrWhiteSpace(model.CustomerMobile))
                                {
                                    x.Span("Phone No: ").FontSize(9);
                                    x.Span(model.CustomerMobile).FontSize(9).Bold();
                                }
                            });
                        });

                        // Items table - reuse existing table structure from InvoiceInStatePdfService
                        col.Item().PaddingTop(6).Element(c =>
                        {
                            c.Table(tableBuilder =>
                            {
                                // Keep the out-of-state invoice columns in the business-required order.
                                tableBuilder.ColumnsDefinition(cd =>
                                {
                                    cd.ConstantColumn(28);    // Sl
                                    cd.RelativeColumn();      // Item Name
                                    cd.ConstantColumn(55);    // HSN
                                    cd.ConstantColumn(48);    // Qty
                                    cd.ConstantColumn(85);    // Rate/Per
                                    cd.ConstantColumn(55);    // UPC
                                    cd.ConstantColumn(55);    // Discount
                                    cd.ConstantColumn(60);    // Taxable
                                    cd.ConstantColumn(48);    // GST
                                    cd.ConstantColumn(70);    // IGST
                                    cd.ConstantColumn(60);    // Total
                                });

                                tableBuilder.Header(header =>
                                {
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("Sl").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("Item Name").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("HSN Code").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("Qty").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("Rate/Per").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("UPC").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("Discount").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("Taxable").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("GST").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("IGST").Bold());
                                    header.Cell().Element(h => h.Border(0.5f).Padding(4).Text("Total").Bold());
                                });

                                foreach (var it in model.Items)
                                {
                                    var displayName = string.IsNullOrWhiteSpace(it.CompanyName) ? it.ItemName : it.CompanyName + " " + it.ItemName;
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.SlNo.ToString()));
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(displayName));
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.HsnCode));
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.Qty.ToString("0.##")).AlignRight());
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.Rate.ToString("0.00") + " / " + it.TypeName).AlignRight());
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.Upc));
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.Discount.ToString("0.##") + " %").AlignRight());
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.Taxable.ToString("0.00")).AlignRight());
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.Igst.ToString("0.00") + " %").AlignRight());
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.IgstAmount.ToString("0.00")).AlignRight());
                                    tableBuilder.Cell().Element(cell => cell.Border(0.5f).Padding(4).Text(it.Total.ToString("0.00")).AlignRight());
                                }

                                // Footer sums for taxable, GST/IGST and total.
                                tableBuilder.Footer(footer =>
                                {
                                    // Span Sl, Item Name, HSN, Qty, Rate/Per, UPC, Discount
                                    footer.Cell().ColumnSpan(7).Element(c => c.Border(0.5f).Padding(4).Text("Total").Bold());
                                    var sumTaxable = model.Items.Sum(x => x.Taxable);
                                    var igstTotal = model.Items.Sum(x => x.IgstAmount);
                                    var sumTotal = model.Items.Sum(x => x.Total);
                                    footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(sumTaxable.ToString("0.00")).AlignRight());
                                    footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(" ").AlignRight());
                                    footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(igstTotal.ToString("0.00")).AlignRight());
                                    footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(sumTotal.ToString("0.00")).AlignRight());
                                });


                            });
                        });

                        // Tax summary and totals - IGST only
                        col.Item().PaddingTop(6).Row(r =>
                        {
                            r.RelativeColumn().Column(left =>
                            {
                                if (model.TaxSummaries != null && model.TaxSummaries.Any())
                                {
                                    left.Item().Width(360).Table(t =>
                                    {
                                        t.ColumnsDefinition(cd =>
                                        {
                                            cd.ConstantColumn(120);
                                            cd.ConstantColumn(80);
                                            cd.ConstantColumn(80);
                                        });

                                        t.Header(h =>
                                        {
                                            h.Cell().Element(c => c.Border(0.5f).Padding(4).Text("Taxable Value").FontSize(9));
                                            h.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text("IGST %").FontSize(9));
                                            h.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text("Amount").FontSize(9));
                                        });

                                        foreach (var ts in model.TaxSummaries)
                                        {
                                            t.Cell().Element(c => c.Border(0.5f).Padding(4).Text(ts.Taxable.ToString("0.00")).FontSize(9));
                                            t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text(ts.IgstPercent.ToString("0.00") + " %").FontSize(9));
                                            t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignRight().Text(ts.IgstAmount.ToString("0.00")).FontSize(9));
                                        }

                                        var totalTaxable = model.TaxSummaries.Sum(x => x.Taxable);
                                        var totalIgst = model.TaxSummaries.Sum(x => x.IgstAmount);

                                        t.Footer(fh =>
                                        {
                                            fh.Cell().Element(c => c.Padding(4).Text(totalTaxable.ToString("0.00")).FontSize(9));
                                            fh.Cell().Element(c => c.Padding(4).Text(" ").FontSize(9));
                                            fh.Cell().Element(c => c.Padding(4).AlignRight().Text(totalIgst.ToString("0.00")).FontSize(9));
                                        });
                                    });
                                }
                            });

                            r.ConstantColumn(220).Column(right =>
                            {
                                // Compute canonical right-side totals. Prefer grouped TaxSummaries if present,
                                // otherwise fall back to per-item sums to avoid mismatches between taxable and total fields.
                                var totalTaxable = (model.TaxSummaries != null && model.TaxSummaries.Any())
                                    ? model.TaxSummaries.Sum(x => x.Taxable)
                                    : model.Items.Sum(x => x.Taxable);

                                var totalIgst = (model.TaxSummaries != null && model.TaxSummaries.Any())
                                    ? model.TaxSummaries.Sum(x => x.IgstAmount)
                                    : model.Items.Sum(x => x.IgstAmount);

                                var invoiceValue = model.InvoiceTotal ?? (totalTaxable + totalIgst);
                                var amountPaid = model.AmountPaid;
                                var balance = model.BalanceAmount;

                                right.Item().AlignRight().Row(rr =>
                                {
                                    rr.RelativeColumn().Text("Taxable value").FontSize(9);
                                    rr.ConstantColumn(120).AlignRight().Text(totalTaxable.ToString("0.00")).FontSize(9);
                                });

                                right.Item().AlignRight().Row(rr =>
                                {
                                    rr.RelativeColumn().Text("IGST").FontSize(9);
                                    rr.ConstantColumn(120).AlignRight().Text(totalIgst.ToString("0.00")).FontSize(9);
                                });

                                right.Item().AlignRight().Row(rr =>
                                {
                                    rr.RelativeColumn().Text("Invoice value").FontSize(9);
                                    rr.ConstantColumn(120).AlignRight().Text(invoiceValue.ToString("0.00")).FontSize(9);
                                });

                                right.Item().AlignRight().Row(rr =>
                                {
                                    rr.RelativeColumn().Text("Amount Paid").FontSize(9);
                                    rr.ConstantColumn(120).AlignRight().Text(amountPaid.ToString("0.00")).FontSize(9);
                                });

                                right.Item().AlignRight().Row(rr =>
                                {
                                    rr.RelativeColumn().Text("Balance").FontSize(9);
                                    rr.ConstantColumn(120).AlignRight().Text(balance.ToString("0.00")).FontSize(9);
                                });
                            });
                        });

                        // Bank details, signature area and footer preserved - simplified for brevity and parity with in-state
                        col.Item().PaddingTop(10).Row(r =>
                        {
                            r.RelativeColumn().Column(left =>
                            {
                                left.Item().PaddingTop(4).Text("Bank Details").Bold().FontSize(9);
                                left.Item().Text(model.BankName + ", " + model.BranchName).FontSize(9);
                                left.Item().Text("A/C No.: " + model.AccountNo).FontSize(9);
                                left.Item().Text("IFSC Code: " + model.Ifsc).FontSize(9);
                            });
                            r.ConstantColumn(220).Column(right =>
                            {
                                right.Item().Column(stampCol =>
                                {
                                    stampCol.Item().AlignCenter().Text("For " + model.CompanyName).Bold().FontSize(10);
                                    stampCol.Item().PaddingTop(35).AlignCenter().Column(p =>
                                    {
                                        p.Item().Text("Proprietor").FontSize(9).AlignRight();
                                    });
                                });
                            });
                        });
                        col.Item().PaddingTop(10).Row(r =>
                        {
                            r.RelativeColumn().Column(left =>
                            {
                                left.Item().Text("Vehicle No: " + model.VehicleNo).FontSize(9);
                                left.Item().Text("Owner Name: " + model.OwnerName).FontSize(9);
                                left.Item().Text("EWay Bill No: " + model.EWayBillNo).FontSize(9);
                            });
                        });
                        col.Item().PaddingTop(8).Column(terms =>
                        {
                            terms.Item().Text("Terms and conditions").Bold().FontSize(9);
                            terms.Item().Text("1. Once product is dispatched it's owner responsibility during transit.").FontSize(8);
                            terms.Item().Text("2. Order estimate cannot be cancelled after receiving payment in advance.").FontSize(8);
                            terms.Item().Text("3. Order can be modified but subjected to availability.").FontSize(8);
                            terms.Item().Text("4. Once packing of goods is done, it cannot be altered.").FontSize(8);
                            terms.Item().Text("5. Any objections on dispatch will be allowed within 24 hours after delivery.").FontSize(8);
                            terms.Item().Text("6. Rates of items will be variable according to current market price.").FontSize(8);
                            terms.Item().Text("7. Delivery of items will be done as per subject of availability. Schedule delivery date as early as possible.").FontSize(8);
                            terms.Item().Text("8. Goods once sold cannot be taken back.").FontSize(8);
                            terms.Item().Text("9. If any discrepancy, it is subject to Rayachoti jurisdiction.").FontSize(8);
                            terms.Item().Text("10. Dispatch of goods will not be entertained one week before Diwali.").FontSize(8);
                            terms.Item().Text("11. Dispatch of goods will be done as per schedule date without any delay.").FontSize(8);
                        });
                    });

                    page.Footer().AlignCenter().Element(f =>
                    {
                        f.Row(r =>
                        {
                            r.RelativeColumn().AlignLeft().Text("Bill No : " + model.InvoiceNo).FontSize(9);
                            r.RelativeColumn().AlignRight().Text(x =>
                            {
                                x.Span("Page ").FontSize(9);
                                x.CurrentPageNumber();
                                x.Span(" of ");
                                x.TotalPages();
                            });
                        });
                    });
                });
            });

            try
            {
                var bytes = doc.GeneratePdf();
                return await Task.FromResult(bytes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Out-of-state PDF generation failed: " + ex.ToString());
                throw;
            }
        }

        public Task<bool> PrintPdfAsync(byte[] pdfBytes, string? printerName = null)
        {
            // Delegate to same simple print approach as in-state service
            try
            {
                var temp = Path.Combine(Path.GetTempPath(), $"pdfprint_{Guid.NewGuid()}.pdf");
                File.WriteAllBytes(temp, pdfBytes);
                var psi = new ProcessStartInfo
                {
                    FileName = temp,
                    UseShellExecute = true,
                    Verb = "print",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var p = Process.Start(psi);
                Task.Delay(3000).ContinueWith(_ =>
                {
                    try { if (p != null && !p.HasExited) p.Kill(); } catch { }
                    try { File.Delete(temp); } catch { }
                });
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PDF print failed: " + ex.Message);
                return Task.FromResult(false);
            }
        }
    }
}
