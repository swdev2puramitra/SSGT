using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using WebApp.Models.Pdf;

namespace WebApp.Services.Pdf
{
    // Renamed: this implementation contains the existing Invoice (In-State) PDF layout
    // Keep a concrete implementation that implements IPdfService so existing DI can resolve it.
    public class InvoiceInStatePdfService : IPdfService
    {
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

        public InvoiceInStatePdfService(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        // Use shared helper for image preparation
        private string PrepareImage(string sourcePath, int targetWidth = 800, int targetHeight = 800)
        {
            return PdfServiceShared.PrepareImage(sourcePath, targetWidth, targetHeight);
        }

        public async Task<byte[]> GeneratePdfAsync(PdfBillModel model, PdfTemplateType template)
        {
            // Use QuestPDF to generate an in-memory PDF
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    switch (template)
                    {
                        case PdfTemplateType.Estimate:
                            page.Size(PageSizes.A4);
                            page.Margin(20);
                            page.DefaultTextStyle(x => x.FontSize(10));
                            break;
                        case PdfTemplateType.InvoiceInState:
                        case PdfTemplateType.InvoiceOutOfState:
                            page.Size(PageSizes.A4.Landscape());
                            page.Margin(18);
                            page.DefaultTextStyle(x => x.FontSize(10));
                            break;
                        case PdfTemplateType.RetailThermal:
                            // Thermal: approximate using A4 portrait with narrow layout. Precise thermal support requires printer-specific handling.
                            page.Size(PageSizes.A4);
                            page.Margin(6);
                            page.DefaultTextStyle(x => x.FontSize(9));
                            break;
                        default:
                            page.Size(PageSizes.A4);
                            page.Margin(20);
                            page.DefaultTextStyle(x => x.FontSize(10));
                            break;
                    }

                    page.Header().Row(row =>
                    {
                        // Unified header used for Estimate and Invoice to keep layouts consistent
                        // Left: logo if available
                        // use a slightly smaller constant column to avoid tight fits; keep image target small
                        row.ConstantColumn(150).AlignMiddle().AlignLeft().PaddingLeft(2).Element(c =>
                        {
                            // Show Licence No on a single row above logo at top-left
                            c.Column(col =>
                            {
                                if (!string.IsNullOrWhiteSpace(model.LicenceNo))
                                    col.Item().Text("Licence No.: " + model.LicenceNo).FontSize(8).AlignLeft();

                                var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? string.Empty, "images", "Ganesha.png");
                                if (System.IO.File.Exists(logoPath))
                                {
                                    // keep prepared image small so it fits header column even when available width is slightly reduced
                                    var safeLogo = PrepareImage(logoPath, 76, 60);
                                    col.Item().Width(76).Height(60).AlignMiddle().AlignLeft().Image(safeLogo);
                                }
                            });
                        });

                        // Center: template title and company details
                        row.RelativeColumn().Column(col =>
                        {
                            if (template == PdfTemplateType.Estimate)
                            {
                                col.Item().PaddingBottom(4).Element(c => c.Text("ESTIMATE").FontSize(12).AlignCenter());
                            }
                            else if (template == PdfTemplateType.InvoiceInState || template == PdfTemplateType.InvoiceOutOfState)
                            {
                                col.Item().PaddingBottom(4).Element(c => c.Text("TAX INVOICE").FontSize(12).AlignCenter());
                            }

                            col.Item().PaddingTop(5).Text(model.CompanyName).Bold().FontSize(22).AlignCenter();
                            col.Item().PaddingTop(5).Text(model.CompanyAddress.Replace("\n", "\n")).FontSize(9f).AlignCenter();
                            col.Item().PaddingTop(5).Text("GSTIN: " + model.Gstin).FontSize(9f).AlignCenter();
                        });

                        // Right: bill meta and small image (remove shop phone here as requested)
                        row.ConstantItem(160).Column( col =>
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
                        // Customer name and mobile on same row
                        col.Item().Row(r2 =>
                        {
                            r2.RelativeColumn().Column(cu =>
                            {
                                cu.Item().Text("Customer: " + model.CustomerName).FontSize(10).Bold();
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
                        // Address on the left and Customer GSTIN on the right, above the items table
                        col.Item().PaddingTop(3).Row(rr =>
                        {
                            rr.RelativeColumn().Text("Address: " + model.CustomerAddress).FontSize(9);
                            rr.ConstantColumn(200).AlignRight().Text(x =>
                            {
                                if (!string.IsNullOrWhiteSpace(model.CustomerGstin))
                                {
                                    x.Span("GSTIN: ").FontSize(9);
                                    x.Span(model.CustomerGstin).FontSize(9).Bold();
                                }
                            });
                        });

                        col.Item().PaddingTop(6).Table(table =>
                        {
                            // define columns to match requested order: Sl No, Item Name, HSN, Qty, Rate/Per, UPC, Discount, Taxable, GST%, CGST Amt, SGST Amt, Total
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(28); // Sl No
                                columns.RelativeColumn(4); // Item Name
                                columns.ConstantColumn(70); // HSN
                                columns.ConstantColumn(48); // Qty
                                columns.ConstantColumn(60); // Rate/Per
                                columns.ConstantColumn(60); // UPC
                                columns.ConstantColumn(50); // Discount
                                columns.ConstantColumn(70); // Taxable
                                columns.ConstantColumn(48); // GST%
                                columns.ConstantColumn(60); // CGST Amt
                                columns.ConstantColumn(60); // SGST Amt
                                columns.ConstantColumn(60); // Total
                            });

                            // header
                            // Use shared TableCellStyle for consistent borders across templates
                            static IContainer CellStyle(IContainer c) => PdfServiceShared.TableCellStyle(c);
                            table.Header(header =>
                            {
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).Text("Sl No");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).Text("Item Name");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).Text("HSN Code");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("Qty");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("Rate/Per");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("UPC");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("Discount %");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("Taxable");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("GST %");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("CGST");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("SGST");
                                header.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text("Total");
                            });

                            // rows
                            foreach (var it in model.Items)
                            {
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).Text(it.SlNo.ToString());                               
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).Text(it.ItemName);
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.HsnCode);
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.NormalizedQuantity.ToString("0.##"));
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.Rate.ToString("0.00") + " / " + it.TypeName);
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.Upc);
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.Discount.ToString("0.##") + " %");
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.Taxable.ToString("0.00"));
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.GstPercent.ToString("0.00") + " %");
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.CgstAmount.ToString("0.00"));
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.SgstAmount.ToString("0.00"));
                                table.Cell().Element(c => PdfServiceShared.TableCellStyle(c)).AlignRight().Text(it.Total.ToString("0.00"));

                                // Use shared TableCellStyle for consistent borders across templates
                            }

                            // After rows, add sum row for taxable, CGST, SGST, Total
                            table.Footer(footer =>
                            {
                                footer.Cell().ColumnSpan(7).Element(c => c.Border(0.5f).Padding(4).Text("Total").Bold());
                                var sumTaxable = model.Items.Sum(x => x.Taxable);
                                var sumCgst = model.Items.Sum(x => x.CgstAmount);
                                var sumSgst = model.Items.Sum(x => x.SgstAmount);
                                var sumTotal = model.Items.Sum(x => x.Total);

                                footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(sumTaxable.ToString("0.00")).AlignRight());
                                footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text("").AlignRight());
                                footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(sumCgst.ToString("0.00")).AlignRight());
                                footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(sumSgst.ToString("0.00")).AlignRight());
                                footer.Cell().Element(c => c.Border(0.5f).Padding(4).Text(sumTotal.ToString("0.00")).AlignRight());
                            });
                        });

                        // For InvoiceInState template, render bank details and tax summary on the left and invoice totals on the right
                        if (template == PdfTemplateType.InvoiceInState || template == PdfTemplateType.InvoiceOutOfState)
                        {
                            col.Item().PaddingTop(6).Row(r =>
                            {
                                // Left column: tax summary + bank details
                                r.RelativeColumn().Column(left =>
                                {
                                    if (model.TaxSummaries != null && model.TaxSummaries.Any())
                                    {
                                        left.Item().Height(6);
                                        left.Item().Width(400).Table(t =>
                                        {
                                            t.ColumnsDefinition(cd =>
                                            {
                                                cd.ConstantColumn(60);
                                                cd.ConstantColumn(38);
                                                cd.ConstantColumn(50);
                                                cd.ConstantColumn(38);
                                                cd.ConstantColumn(50);
                                                cd.ConstantColumn(60);
                                            });

                                            t.Header(h =>
                                            {
                                                h.Cell().Element(c => c.Border(0.5f).Padding(4).Text("Taxable Value").FontSize(9));
                                                h.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text("CGST %").FontSize(9));
                                                h.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text("Amount").FontSize(9));
                                                h.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text("SGST %").FontSize(9));
                                                h.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text("Amount").FontSize(9));
                                                h.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text("Total Tax").FontSize(9));
                                            });

                                            foreach (var ts in model.TaxSummaries)
                                            {
                                                t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignRight().Text(ts.Taxable.ToString("0.00")).FontSize(9));
                                                t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text(ts.CgstPercent.ToString("0.00") + " %").FontSize(9));
                                                t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignRight().Text(ts.CgstAmount.ToString("0.00")).FontSize(9));
                                                t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignCenter().Text(ts.SgstPercent.ToString("0.00") + " %").FontSize(9));
                                                t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignRight().Text(ts.SgstAmount.ToString("0.00")).FontSize(9));
                                                t.Cell().Element(c => c.Border(0.5f).Padding(4).AlignRight().Text(ts.TotalTax.ToString("0.00")).FontSize(9));
                                            }

                                            var totalTaxable = model.TaxSummaries.Sum(x => x.Taxable);
                                            var totalCgst = model.TaxSummaries.Sum(x => x.CgstAmount);
                                            var totalSgst = model.TaxSummaries.Sum(x => x.SgstAmount);
                                            var totalTax = model.TaxSummaries.Sum(x => x.TotalTax);

                                            t.Footer(fh =>
                                            {
                                                fh.Cell().Element(c => c.Padding(4).AlignRight().Text(totalTaxable.ToString("0.00")).FontSize(9));
                                                fh.Cell().Element(c => c.Padding(4).Text(" ").FontSize(9));
                                                fh.Cell().Element(c => c.Padding(4).AlignRight().Text(totalCgst.ToString("0.00")).FontSize(9));
                                                fh.Cell().Element(c => c.Padding(4).Text(" ").FontSize(9));
                                                fh.Cell().Element(c => c.Padding(4).AlignRight().Text(totalSgst.ToString("0.00")).FontSize(9));
                                                fh.Cell().Element(c => c.Padding(4).AlignRight().Text(totalTax.ToString("0.00")).FontSize(9));
                                            });
                                        });

                                        left.Item().Height(6);
                                    }
                                });

                                // Right column: totals and stamp area
                                r.ConstantColumn(220).Column(right =>
                                {
                                    right.Item().AlignRight().Row(rr =>
                                    {
                                        rr.RelativeColumn().Text("Taxable value").FontSize(9);
                                        rr.ConstantColumn(120).AlignRight().Text(model.TaxSummaries != null && model.TaxSummaries.Any() ? model.TaxSummaries.Sum(x => x.Taxable).ToString("0.00") : model.TotalAmount.ToString("0.00")).Bold().FontSize(10);
                                    });

                                    right.Item().PaddingTop(4).Row(rr =>
                                    {
                                        rr.RelativeColumn().Text("CGST").FontSize(9);
                                        rr.ConstantColumn(120).AlignRight().Text(model.TaxSummaries != null && model.TaxSummaries.Any() ? model.TaxSummaries.Sum(x => x.CgstAmount).ToString("0.00") : "0.00").FontSize(9);
                                    });

                                    right.Item().PaddingTop(2).Row(rr =>
                                    {
                                        rr.RelativeColumn().Text("SGST").FontSize(9);
                                        rr.ConstantColumn(120).AlignRight().Text(model.TaxSummaries != null && model.TaxSummaries.Any() ? model.TaxSummaries.Sum(x => x.SgstAmount).ToString("0.00") : "0.00").FontSize(9);
                                    });

                                    right.Item().PaddingTop(6).Column(colu =>
                                    {
                                        colu.Item().Row(rr =>
                                        {
                                            rr.RelativeColumn().Text("Invoice value").Bold().FontSize(10);
                                            rr.ConstantColumn(120).AlignRight().Text(x =>
                                            {
                                                var invoiceValue = model.InvoiceTotal ?? (model.TotalAmount + (model.TaxSummaries != null ? model.TaxSummaries.Sum(x => x.TotalTax) : 0m));
                                                x.Span(invoiceValue.ToString("0.00")).Bold().FontSize(10);
                                            });
                                        });

                                        // Show Amount Paid (advance) and Balance (Total - Paid)
                                        colu.Item().PaddingTop(6).Row(rrp =>
                                        {
                                            rrp.RelativeColumn().Text("Amount Paid").FontSize(9);
                                            rrp.ConstantColumn(120).AlignRight().Text((model.AmountPaid).ToString("0.00")).FontSize(9);
                                        });

                                        colu.Item().Row(rrb =>
                                        {
                                            rrb.RelativeColumn().Text("Balance").Bold().FontSize(10);
                                            rrb.ConstantColumn(120).AlignRight().Text((model.BalanceAmount).ToString("0.00")).Bold().FontSize(10);
                                        });

                                        colu.Item().Container().BorderBottom(1.2f).BorderColor(Colors.Black).Height(2);
                                        colu.Item().Container().BorderBottom(0.6f).BorderColor(Colors.Black).Height(2);
                                    });
                                });
                            });
                            col.Item().PaddingTop(8).Row(r =>
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
                            col.Item().PaddingTop(8).Row(r =>
                            {
                                r.RelativeColumn().Column(left =>
                                {
                                    if (template == PdfTemplateType.InvoiceInState)
                                    {
                                        left.Item().Text("Vehicle No: " + model.VehicleNo).FontSize(9);
                                        left.Item().Text("Driver/Owner Name: " + model.OwnerName).FontSize(9);
                                        left.Item().Text("EWay Bill No: " + model.EWayBillNo).FontSize(9);
                                    }
                                });
                            });
                        }
                    });

                    page.Footer().Element(f =>
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
                // Write full exception including QuestPDF layout trace to debug output to help diagnose single-child container errors
                Debug.WriteLine("PDF generation failed: " + ex.ToString());
                throw;
            }
        }

        public Task<bool> PrintPdfAsync(byte[] pdfBytes, string? printerName = null)
        {
            try
            {
                // Write to temp file and invoke default PDF viewer print verb - best-effort on Windows
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

                if (!string.IsNullOrEmpty(printerName))
                {
                    // Some viewers support /p /h arguments or specifying printer via command line - not standardized
                }

                Process? p = Process.Start(psi);

                // Do not wait indefinitely - give the print job some time then attempt to delete
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
