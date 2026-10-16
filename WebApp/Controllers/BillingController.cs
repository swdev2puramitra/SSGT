using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using WebApp.Helpers;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class BillingController : BaseController
    {
        private readonly SsgtContext _context;
        private readonly WebApp.Services.Pdf.IPdfService _pdfService;
        private readonly IErrorLogService _errorLogService;
        private readonly ItemQuantityConversionHelper _quantityConversionHelper;

        public BillingController(SsgtContext context, WebApp.Services.Pdf.IPdfService pdfService, IErrorLogService errorLogService, ItemQuantityConversionHelper quantityConversionHelper)
        {
            _context = context;
            _pdfService = pdfService;
            _errorLogService = errorLogService;
            _quantityConversionHelper = quantityConversionHelper;
        }

        // GET: /Billing/PreviewPdf?billId=123&template=InvoiceInState
        [HttpGet]
        // Default to Estimate template for previews when not specified
        public async System.Threading.Tasks.Task<IActionResult> PreviewPdf(long billId, string template = "Estimate")
        {
            try
            {
                // Start model with defaults; will populate from bill or invoice below
                var model = new WebApp.Models.Pdf.PdfBillModel
                {
                    CompanyName = "Your Company Name",
                    CompanyAddress = "Address Line 1, City",
                    Gstin = string.Empty,
                    BillNo = string.Empty,
                    BillDate = DateTime.Now,
                    CustomerName = string.Empty,
                    CustomerAddress = string.Empty,
                    IsInState = true,
                    TotalAmount = 0m
                };

                // Determine whether to load an estimate (FW_Bills) or an invoice (FW_Invoices) based on template
                var templateNorm = (template ?? string.Empty).Trim();
                // we'll load HSN codes in bulk per document to avoid N+1 queries
                if (templateNorm.Equals("InvoiceInState", StringComparison.OrdinalIgnoreCase) || templateNorm.Equals("InvoiceOutOfState", StringComparison.OrdinalIgnoreCase))
                {
                    // Load invoice and its items
                    var inv = _context.FwInvoices.AsNoTracking()
                        .Include(i => i.FwInvoiceItems).ThenInclude(ii => ii.Item)
                        .Include(i => i.FwInvoiceItems).ThenInclude(ii => ii.ItemType)
                        .FirstOrDefault(i => i.InvoiceId == billId || i.BillId == billId);
                    if (inv == null) return NotFound();

                    model.BillNo = inv.BillNo ?? inv.InvoiceNo ?? string.Empty;
                    model.BillDate = inv.CreatedDate;
                    model.IsInState = inv.IsInState ?? true;
                    model.TotalAmount = (inv.TotalBillAmount ?? inv.BillAmount);
                    // include amount paid (advance) and balance for PDF
                    model.AmountPaid = inv.AmountPaid ?? 0m;
                    model.BalanceAmount = Convert.ToDecimal(inv.BalanceAmount);
                    model.InvoiceNo = inv.InvoiceNo;
                    model.VehicleNo = inv.VehicleNo ?? string.Empty;
                    model.OwnerName = inv.OwnerName ?? string.Empty;
                    model.EWayBillNo = inv.WayBill ?? string.Empty;
                    // Customer details from invoice or linked bill
                    try
                    {
                        if (inv.CustomerId != 0)
                        {
                            var cust = _context.FwMasterCustomers.AsNoTracking().FirstOrDefault(c => c.CustomerId == inv.CustomerId);
                            if (cust != null)
                            {
                                model.CustomerName = cust.Name ?? model.CustomerName;
                                model.CustomerAddress = cust.CustAddress ?? model.CustomerAddress;
                                model.CustomerMobile = cust.MobileNo.HasValue ? cust.MobileNo.Value.ToString() : string.Empty;
                            }
                        }
                    }
                    catch { }

                    // Build mapping ItemId -> HsnCode by joining master items to master HSNs
                    var itemIds = inv.FwInvoiceItems.Select(x => x.ItemId).Distinct().ToList();
                    var itemsData = _context.FwMasterItems.AsNoTracking()
                        .Where(mi => itemIds.Contains(mi.ItemId))
                        .Select(mi => new { mi.ItemId, mi.Hsnid })
                        .ToList();

                    var itemHsnIds = itemsData.Select(i => i.Hsnid).Where(x => x.HasValue).Select(x => (long)x.Value).Distinct().ToList();
                    var hsnMap = itemHsnIds.Any()
                        ? _context.FwMasterHsns.AsNoTracking().Where(h => itemHsnIds.Contains(h.Hsnid)).ToDictionary(h => h.Hsnid, h => h.Hsncode)
                        : new System.Collections.Generic.Dictionary<long, string>();

                    var itemToHsn = itemsData.ToDictionary(i => i.ItemId, i => i.Hsnid.HasValue && hsnMap.ContainsKey((long)i.Hsnid.Value) ? hsnMap[(long)i.Hsnid.Value] : string.Empty);

                    // Build a map of qtyPerType for item+type so we can compute display quantities (type-units)
                    var conversionRows = _context.FwMasterItemsConversions.AsNoTracking()
                        .Where(c => itemIds.Contains(c.ItemId))
                        .Select(c => new { c.ItemId, c.ItemTypeId, c.QtyPerType })
                        .ToList();

                    var convMap = conversionRows.GroupBy(c => c.ItemId)
                        .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.ItemTypeId, x => x.QtyPerType));

                    // Prefer per-item DiscountPerItem if present: treat it as percentage per displayed type-unit (percent)
                    var invDiscountPercent = inv.DiscountPerItem ?? 0m;
                    if (invDiscountPercent < 0m) invDiscountPercent = 0m;
                    if (invDiscountPercent > 100m) invDiscountPercent = 100m;

                    var taxGroup = new List<WebApp.Models.Pdf.PdfTaxSummary>();
                    foreach (var it in (inv.FwInvoiceItems ?? new List<WebApp.Models.FwInvoiceItem>()).OrderBy(item => item.SlNo))
                    {
                        var hsn = itemToHsn != null && itemToHsn.ContainsKey(it.ItemId) ? itemToHsn[it.ItemId] : string.Empty;

                        // base units (stored in DB) - quantity is stored as lowest-level/base units
                        var baseQty = it.Quantity;

                        // qtyPerType: number of base units that make one displayed type-unit (e.g., units per box)
                        int qtyPerType = 1;
                        try
                        {
                            if (convMap.ContainsKey(it.ItemId) && convMap[it.ItemId].ContainsKey(it.ItemTypeId))
                                qtyPerType = convMap[it.ItemId][it.ItemTypeId] ?? 1;
                        }
                        catch { qtyPerType = 1; }
                        if (qtyPerType <= 0) qtyPerType = 1;

                        // display quantity shown to user = base units / qtyPerType
                        var displayQty = (decimal)baseQty / (decimal)qtyPerType;

                        // determine unit rate per base unit: prefer ChangedPrice when price was modified, else OriginalCost if present, then ItemCost
                        var unitRatePerBase = (it.ChangedPrice.HasValue && (it.IsPriceModified ?? false))
                            ? it.ChangedPrice.Value
                            : (it.OriginalCost.HasValue ? it.OriginalCost.Value : it.ItemCost);

                        // compute rate per displayed type-unit
                        var ratePerType = unitRatePerBase * qtyPerType;

                        // apply percentage discount per type-unit (DiscountPerItem is percent)
                        var discountedRatePerType = ratePerType * (1 - (invDiscountPercent / 100m));
                        if (discountedRatePerType < 0m) discountedRatePerType = 0m;

                        var taxable = Math.Round(displayQty * discountedRatePerType, 2);

                        // Interpret stored tax fields (Cgst/Sgst/Igst) as percentages when they look like rates (0-100).
                        // Prefer percent-based storage: if value is between 0 and 100 treat as percent, else treat as absolute amount.
                        decimal cgstAmt = 0m, sgstAmt = 0m, igstAmt = 0m;
                        decimal cgstPercent = 0m, sgstPercent = 0m, igstPercent = 0m;

                        if (it.Cgst.HasValue)
                        {
                            var v = it.Cgst.Value;
                            if (v >= 0m && v <= 100m)
                            {
                                cgstPercent = v;
                                cgstAmt = Math.Round(taxable * cgstPercent / 100m, 2);
                            }
                            else
                            {
                                cgstAmt = v;
                                cgstPercent = taxable > 0m ? Math.Round(cgstAmt * 100m / taxable, 2) : 0m;
                            }
                        }

                        if (it.Sgst.HasValue)
                        {
                            var v = it.Sgst.Value;
                            if (v >= 0m && v <= 100m)
                            {
                                sgstPercent = v;
                                sgstAmt = Math.Round(taxable * sgstPercent / 100m, 2);
                            }
                            else
                            {
                                sgstAmt = v;
                                sgstPercent = taxable > 0m ? Math.Round(sgstAmt * 100m / taxable, 2) : 0m;
                            }
                        }

                        if (it.Igst.HasValue)
                        {
                            var v = it.Igst.Value;
                            if (v >= 0m && v <= 100m)
                            {
                                igstPercent = v;
                                igstAmt = Math.Round(taxable * igstPercent / 100m, 2);
                            }
                            else
                            {
                                igstAmt = v;
                                igstPercent = taxable > 0m ? Math.Round(igstAmt * 100m / taxable, 2) : 0m;
                            }
                        }

                        // Split item name into CompanyName (first token) and remaining ItemName
                        var rawItemName = it.Item?.ItemName ?? "Item";
                        var tokens = (rawItemName ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var compName = tokens.Length > 0 ? tokens[0] : string.Empty;
                        var itemNameRest = tokens.Length > 1 ? string.Join(' ', tokens.Skip(1)) : rawItemName;

                        model.Items.Add(new WebApp.Models.Pdf.PdfBillItem
                        {
                            SlNo = it.SlNo ?? 0,
                            CompanyName = compName,
                            ItemName = itemNameRest,
                            HsnCode = hsn,
                            TypeName = it.ItemType?.TypeName ?? string.Empty,
                            Qty = displayQty,
                            NormalizedQuantity = baseQty,
                            OriginalEntry = it.SellQty.HasValue && it.SellQty.Value > 0
                                ? $"{it.SellQty.Value} {it.ItemType?.TypeName ?? "Unit"}'s"
                                : string.Empty,
                            Rate = Math.Round(unitRatePerBase, 2),
                            // Keep Discount percent empty (0) because DiscountPerItem is absolute amount per item
                            Discount = 0m,
                            Taxable = taxable,
                            GstPercent = cgstPercent + sgstPercent + igstPercent,
                            Cgst = cgstPercent,
                            Sgst = sgstPercent,
                            Igst = igstPercent,
                            CgstAmount = cgstAmt,
                            SgstAmount = sgstAmt,
                            IgstAmount = igstAmt,
                            Upc = qtyPerType.ToString(),
                            Total = Math.Round(taxable + cgstAmt + sgstAmt + igstAmt, 2)
                        });

                        // accumulate into tax groups by CGST/SGST/IGST percentages
                        var existing = taxGroup.FirstOrDefault(t => t.CgstPercent == cgstPercent && t.SgstPercent == sgstPercent && t.IgstPercent == igstPercent);
                        if (existing == null)
                        {
                            taxGroup.Add(new WebApp.Models.Pdf.PdfTaxSummary
                            {
                                Taxable = taxable,
                                CgstPercent = cgstPercent,
                                CgstAmount = cgstAmt,
                                SgstPercent = sgstPercent,
                                SgstAmount = sgstAmt,
                                IgstPercent = igstPercent,
                                IgstAmount = igstAmt
                            });
                        }
                        else
                        {
                            existing.Taxable += taxable;
                            existing.CgstAmount += cgstAmt;
                            existing.SgstAmount += sgstAmt;
                            existing.IgstAmount += igstAmt;
                        }
                    }

                    // assign grouped tax summaries to model
                    model.TaxSummaries = taxGroup.OrderBy(t => t.CgstPercent).ThenBy(t => t.SgstPercent).ToList();

                    // Recompute canonical totals from item lines to avoid relying on possibly stale DB totals
                    // TotalAmount represents taxable value (sum of per-line taxable) for rendering
                    model.TotalAmount = Math.Round(model.Items.Sum(x => x.Taxable), 2);

                    // populate shop and customer GST/phone/branch
                    var shop = _context.FwShopDetails.AsNoTracking().FirstOrDefault();
                    if (shop != null)
                    {
                        model.CompanyName = shop.ShopName ?? model.CompanyName;
                        model.CompanyAddress = shop.Address ?? model.CompanyAddress;
                        model.Gstin = shop.Gst ?? model.Gstin;
                        model.LicenceNo = shop.LicenceNo ?? model.LicenceNo;
                        model.PhoneNo = shop.PhoneNo ?? model.PhoneNo;
                        model.BankName = shop.BankName ?? model.BankName;
                        model.AccountNo = shop.AccountNo ?? model.AccountNo;
                        model.Ifsc = shop.Ifsc ?? model.Ifsc;
                        model.BranchName = shop.BranchName ?? model.BranchName;
                    }
                    // customer GST if present on invoice
                    model.CustomerGstin = inv.CustGstin ?? string.Empty;
                }
                else
                {
                    // Load estimate from FW_Bills
                    var bill = _context.FwBills.AsNoTracking()
                        .Include(b => b.FwBillsItems).ThenInclude(i => i.Item)
                        .Include(b => b.FwBillsItems).ThenInclude(i => i.ItemType)
                        .FirstOrDefault(b => b.BillId == billId);
                    if (bill == null) return NotFound();

                    model.BillNo = bill.BillNo ?? string.Empty;
                    model.BillDate = bill.CreatedDate;
                    model.IsInState = bill.IsInState ?? true;
                    // Don't rely on potentially stale DB totals; recompute from items to match client-side totals

                    // Customer details from bill
                    try
                    {
                        if (bill.CustomerId != 0)
                        {
                            var cust = _context.FwMasterCustomers.AsNoTracking().FirstOrDefault(c => c.CustomerId == bill.CustomerId);
                            if (cust != null)
                            {
                                model.CustomerName = cust.Name ?? model.CustomerName;
                                model.CustomerAddress = cust.CustAddress ?? model.CustomerAddress;
                                model.CustomerMobile = cust.MobileNo?.ToString();
                            }
                        }
                    }
                    catch { }

                    // Build mapping ItemId -> HsnCode by joining master items to master HSNs for this bill
                    var billItemIds = bill.FwBillsItems.Select(x => x.ItemId).Distinct().ToList();
                    var billItemsData = _context.FwMasterItems.AsNoTracking()
                        .Where(mi => billItemIds.Contains(mi.ItemId))
                        .Select(mi => new { mi.ItemId, mi.Hsnid })
                        .ToList();

                    var billItemHsnIds = billItemsData.Select(i => i.Hsnid).Where(x => x.HasValue).Select(x => (long)x.Value).Distinct().ToList();
                    var billHsnMap = billItemHsnIds.Any()
                        ? _context.FwMasterHsns.AsNoTracking().Where(h => billItemHsnIds.Contains(h.Hsnid)).ToDictionary(h => h.Hsnid, h => h.Hsncode)
                        : new System.Collections.Generic.Dictionary<long, string>();

                    var billItemToHsn = billItemsData.ToDictionary(i => i.ItemId, i => i.Hsnid.HasValue && billHsnMap.ContainsKey((long)i.Hsnid.Value) ? billHsnMap[(long)i.Hsnid.Value] : string.Empty);

                    // Build a map of qtyPerType for item+type so we can compute display quantities (type-units)
                    var conversionRows = _context.FwMasterItemsConversions.AsNoTracking()
                        .Where(c => billItemIds.Contains(c.ItemId))
                        .Select(c => new { c.ItemId, c.ItemTypeId, c.QtyPerType })
                        .ToList();

                    var convMap = conversionRows.GroupBy(c => c.ItemId)
                        .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.ItemTypeId, x => x.QtyPerType));

                    // discountPerItem in UI is a percentage per displayed type-unit (0-100)
                    var billDiscountPercent = bill.DiscountPerItem ?? 0m;
                    if (billDiscountPercent < 0m) billDiscountPercent = 0m;
                    if (billDiscountPercent > 100m) billDiscountPercent = 100m;
                    var taxGroupBill = new List<WebApp.Models.Pdf.PdfTaxSummary>();

                    foreach (var it in (bill.FwBillsItems ?? new List<WebApp.Models.FwBillsItem>()).OrderBy(item => item.SlNo))
                    {
                        var unitRatePerBase = (it.ChangedPrice.HasValue && (it.IsPriceModified ?? false))
                            ? it.ChangedPrice.Value
                            : (it.OriginalCost.HasValue ? it.OriginalCost.Value : it.ItemCost);
                        var normalizedQty = it.SellQty.HasValue && it.SellQty.Value > 0 && it.SellType.HasValue && it.SellType.Value > 0
                            ? _quantityConversionHelper.ConvertToLowestLevel(it.ItemId, it.SellType.Value, it.SellQty.Value, unitRatePerBase).normalizedQty
                            : it.Quantity;
                        var taxable = Math.Round(normalizedQty * unitRatePerBase, 2, MidpointRounding.AwayFromZero);

                        var lineTotal = Math.Round(taxable, 2);

                        // Split item name into CompanyName (first token) and remaining ItemName for estimates
                        var rawItemNameBill = it.Item?.ItemName ?? "Item";
                        var tokensBill = (rawItemNameBill ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var compNameBill = tokensBill.Length > 0 ? tokensBill[0] : string.Empty;
                        var itemNameRestBill = tokensBill.Length > 1 ? string.Join(' ', tokensBill.Skip(1)) : rawItemNameBill;

                        model.Items.Add(new WebApp.Models.Pdf.PdfBillItem
                        {
                            SlNo = it.SlNo ?? 0,
                            CompanyName = compNameBill,
                            ItemName = itemNameRestBill,
                            TypeName = it.ItemType?.TypeName ?? string.Empty,
                            Qty = normalizedQty,
                            NormalizedQuantity = normalizedQty,
                            OriginalEntry = it.SellQty.HasValue && it.SellQty.Value > 0
                                ? $"{it.SellQty.Value} {it.ItemType?.TypeName ?? "Unit"}'s"
                                : string.Empty,
                            Rate = Math.Round(unitRatePerBase, 2),
                            // Discount stored at header level as DiscountPerItem (absolute amount). Keep item-level percent 0
                            Discount = 0m,
                            Taxable = taxable,
                            Total = lineTotal
                        });
                    }

                    model.TaxSummaries = taxGroupBill.OrderBy(t => t.CgstPercent).ThenBy(t => t.SgstPercent).ToList();

                    // TotalAmount represents taxable value (sum of per-line taxable) for rendering
                    model.TotalAmount = Math.Round(model.Items.Sum(x => x.Taxable), 2);
                }

                // Fill shop/company details from FW_ShopDetails (single row expected)
                try
                {
                    var shop = _context.FwShopDetails.AsNoTracking().FirstOrDefault();
                    if (shop != null)
                    {
                        model.CompanyName = shop.ShopName ?? model.CompanyName;
                        model.CompanyAddress = shop.Address ?? model.CompanyAddress;
                        model.Gstin = shop.Gst ?? model.Gstin;
                        model.LicenceNo = shop.LicenceNo ?? string.Empty;
                        model.PhoneNo = shop.PhoneNo ?? string.Empty;
                        model.BankName = shop.BankName ?? string.Empty;
                        model.AccountNo = shop.AccountNo ?? string.Empty;
                        model.Ifsc = shop.Ifsc ?? string.Empty;
                        // add phone/licence into address or separate fields if needed
                        if (!string.IsNullOrWhiteSpace(shop.PhoneNo))
                        {
                            model.CompanyAddress = (model.CompanyAddress + "\nPhone: " + shop.PhoneNo).Trim();
                        }
                    }
                }
                catch { }

                // (Customer and item population already handled above based on selected template)
                if (!Enum.TryParse<WebApp.Services.Pdf.PdfTemplateType>(template, out var ttype))
                    ttype = WebApp.Services.Pdf.PdfTemplateType.Estimate;

                var bytes = await _pdfService.GeneratePdfAsync(model, ttype);
                return File(bytes, "application/pdf", fileDownloadName: null);
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.PreviewPdf",
                    $"BillId={billId}, Template={template}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error generating pdf preview: {ex.Message}");
                return StatusCode(500);
            }
        }

        // GET: /Billing/GetCustomerByMobile?mobile=9999999999
        [HttpGet]
        public async Task<IActionResult> GetCustomerByMobile(long mobile)
        {
            try
            {
                if (mobile <= 0) return Json(new { success = false, message = "Invalid mobile" });

                var cust = await _context.FwMasterCustomers.AsNoTracking().FirstOrDefaultAsync(c => c.MobileNo == mobile);
                if (cust == null)
                    return Json(new { success = true, found = false });

                return Json(new
                {
                    success = true,
                    found = true,
                    data = new
                    {
                        customerId = cust.CustomerId,
                        name = cust.Name,
                        customerType = cust.CustomerType,
                        custAddress = cust.CustAddress,
                        custGstin = (cust.GetType().GetProperty("CustGstin") != null) ? (string?)cust.GetType().GetProperty("CustGstin").GetValue(cust) : null,
                        mobileNo = cust.MobileNo,
                        mobile2 = cust.Mobile2
                    }
                });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.GetCustomerByMobile",
                    $"Mobile={mobile}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error fetching customer: {ex.Message}");
                return Json(new { success = false, message = "Error fetching customer" });
            }
        }

        // POST: /Billing/CreateCustomer
        public class CustomerCreateDto { public string? Name { get; set; } public long MobileNo { get; set; } public byte CustomerType { get; set; } public string? CustAddress { get; set; } }

        // GET: /Billing/GetItemTypes?itemId=123
        [HttpGet]
        public async Task<IActionResult> GetItemTypes(int itemId)
        {
            try
            {
                if (itemId <= 0)
                    return Json(new { success = true, data = new List<ItemTypeDto>() });

                var param = new SqlParameter("@ItemId", System.Data.SqlDbType.Int) { Value = itemId };

                // Use EF LINQ to query conversions and item types
                var list = await (from c in _context.FwMasterItemsConversions
                                  join i in _context.FwMasterItemTypes on c.ItemTypeId equals i.Id
                                  where c.ItemId == itemId && c.IsActive == true && i.IsActive == true
                                  select new ItemTypeDto
                                  {
                                      ItemTypeId = c.ItemTypeId,
                                      TypeName = i.TypeName,
                                      QtyPerType = c.QtyPerType ?? 0,
                                      IsActive = c.IsActive == true ? "Yes" : "No"
                                  })
                           .AsNoTracking()
                           .ToListAsync();

                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.GetItemTypes",
                    $"ItemId={itemId}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error fetching item types: {ex.Message}");
                return Json(new { success = false, message = "Error fetching item types" });
            }
        }

        // GET: /Billing/GetItemTypePrice?itemId=123&itemTypeId=4&customerType=1
        [HttpGet]
        public async Task<IActionResult> GetItemTypePrice(int itemId, int itemTypeId, int customerType)
        {
            try
            {
                if (itemId <= 0 || itemTypeId <= 0)
                    return Json(new { success = false, message = "Invalid item or type" });

                var conv = await _context.FwMasterItemsConversions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.ItemId == itemId && c.ItemTypeId == itemTypeId && c.IsActive == true);

                if (conv == null)
                    return Json(new { success = false, message = "Item type conversion not found" });

                decimal price = 0m;
                switch (customerType)
                {
                    case 1:
                        price = conv.WholeSale;
                        break;
                    case 2:
                        price = conv.SemiWholeSale;
                        break;
                    case 3:
                        price = conv.Retail;
                        break;
                    default:
                        price = conv.Retail;
                        break;
                }

                return Json(new { success = true, price = price, qtyPerType = conv.QtyPerType ?? 0 });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.GetItemTypePrice",
                    $"ItemId={itemId}, ItemTypeId={itemTypeId}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error fetching item type price: {ex.Message}");
                return Json(new { success = false, message = "Error fetching price" });
            }
        }

        // GET: /Billing/GetCustomerTypes
        [HttpGet]
        public async Task<IActionResult> GetCustomerTypes()
        {
            try
            {
                // Read customer types from new master table FW_Master_CustomerType
                var dbList = await _context.FwMasterCustomerTypes
                    .AsNoTracking()
                    .Where(t => t.IsActive == true)
                    .OrderBy(t => t.TypeId)
                    .Select(t => new { id = t.TypeId, name = t.CustTypeName })
                    .ToListAsync();

                return Json(new { success = true, data = dbList });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.GetCustomerTypes",
                    null,
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error fetching customer types: {ex.Message}");
                return Json(new { success = false, message = "Error fetching customer types" });
            }
        }

        // GET: /Billing/StockTransfer
        public IActionResult StockTransfer(int? transferId)
        {
            LoadMenusFromSession();

            var godowns = _context.FwMasterGodowns
                .Where(g => g.IsActive == null || g.IsActive == true)
                .OrderBy(g => g.GodownDescription)
                .ToList();

            ViewBag.Godowns = godowns;
            ViewBag.Today = DateTime.Now.ToString("dd MMM yyyy");

            // If a transferId is provided, load transfer header and items to prefill the edit form
            if (transferId.HasValue && transferId.Value > 0)
            {
                var transfer = _context.FwStockTransfers
                    .Include(t => t.Items)
                    .ThenInclude(i => i.Item)
                    .FirstOrDefault(t => t.TransferId == transferId.Value);

                if (transfer != null)
                {
                    // Build item types map for prefill and convert stored (lowest-level) quantity back to item-type quantity
                    var itemTypesMap = new Dictionary<string, List<object>>();
                    var itemsDto = new List<object>();

                    var idx = 0;
                    foreach (var it in transfer.Items)
                    {
                        idx++;
                        // fetch lowest-level info for this item+type
                        var p1 = new SqlParameter("@ItemId", System.Data.SqlDbType.Int) { Value = it.ItemId };
                        var p2 = new SqlParameter("@ItemTypeId", System.Data.SqlDbType.Int) { Value = (object?)it.ItemTypeId ?? 0 };
                        var p3 = new SqlParameter("@CustomerType", System.Data.SqlDbType.Int) { Value = 1 };

                        var rows = _context.Set<ItemLowestPriceDto>().FromSqlRaw("EXEC USP_GET_ItemLowestPrice @ItemId, @ItemTypeId, @CustomerType", p1, p2, p3).AsNoTracking().ToList();

                        decimal displayPrice = 0m;
                        int storedLowest = (it.QtyLowest > 0) ? it.QtyLowest : it.Quantity; // support older rows where QtyLowest may not exist
                        bool hasQtyLowest = (it.QtyLowest > 0);
                        int displayQty = it.Quantity;

                        if (rows != null && rows.Count > 0)
                        {
                            // Prefer the row that matches the stored ItemTypeId so QtyPerType is correct for conversion
                            ItemLowestPriceDto first = null;
                            if (it.ItemTypeId > 0)
                            {
                                first = rows.FirstOrDefault(r => r.ItemTypeId == it.ItemTypeId);
                            }
                            if (first == null) first = rows[0];

                            displayPrice = first.Price;
                            if (first.QtyPerType > 0 && hasQtyLowest)
                            {
                                try { displayQty = storedLowest / first.QtyPerType; }
                                catch { displayQty = storedLowest; }
                            }
                        }

                        // fetch available item types for this item to populate the client-side selector
                        var types = (from c in _context.FwMasterItemsConversions
                                     join i in _context.FwMasterItemTypes on c.ItemTypeId equals i.Id
                                     where c.ItemId == it.ItemId && c.IsActive == true && i.IsActive == true
                                     select new { itemTypeId = c.ItemTypeId, typeName = i.TypeName, qtyPerType = c.QtyPerType }).AsNoTracking().ToList();

                        var key = it.ItemId.ToString();
                        if (!itemTypesMap.ContainsKey(key))
                            itemTypesMap[key] = types.Select(t => (object)new { itemTypeId = t.itemTypeId, typeName = t.typeName }).ToList();

                        itemsDto.Add(new
                        {
                            sl = idx,
                            itemId = it.ItemId,
                            itemName = it.Item != null ? it.Item.ItemName : string.Empty,
                            itemPrice = displayPrice,
                            itemTypeId = it.ItemTypeId,
                            quantity = displayQty,
                            qtyLowest = storedLowest
                        });
                    }

                    var transferObj = new
                    {
                        fromGodown = transfer.FromGodown,
                        toGodown = transfer.ToGodown,
                        items = itemsDto,
                        itemTypes = itemTypesMap
                    };

                    ViewBag.TransferJson = System.Text.Json.JsonSerializer.Serialize(transferObj);
                }
                else
                {
                    ViewBag.TransferJson = null;
                }
            }
            else
            {
                ViewBag.TransferJson = null;
            }

            return View();
        }

        // GET: /Billing/FireWorksBilling
        public IActionResult FireWorksBilling(long? billId)
        {
            LoadMenusFromSession();

            // Restrict access to edit view for non-admin users when a billId is provided
            try
            {
                var roleClaimValue = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                int.TryParse(roleClaimValue, out var userRole);
                if (billId.HasValue && billId.Value > 0 && userRole != 1)
                {
                    // Non-admins should not access the edit screen
                    return RedirectToAction("AccessDenied", "Home");
                }
            }
            catch { }

            // If billId provided, load bill/header and items for editing and pass as JSON to the view
            if (billId.HasValue && billId.Value > 0)
            {
                try
                {
                    // Check if an invoice exists for this bill
                    var invoice = _context.FwInvoices.AsNoTracking().FirstOrDefault(i => i.BillId == billId.Value);
                    if (invoice != null)
                    {
                        var header = new
                        {
                            billId = invoice.BillId,
                            billNo = invoice.BillNo,
                            customerId = invoice.CustomerId,
                            name = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == invoice.CustomerId).Select(c => c.Name).FirstOrDefault(),
                            custAddress = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == invoice.CustomerId).Select(c => c.CustAddress).FirstOrDefault(),
                            mobileNo = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == invoice.CustomerId).Select(c => c.MobileNo).FirstOrDefault(),
                            mobile2 = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == invoice.CustomerId).Select(c => c.Mobile2).FirstOrDefault(),
                            customerType = invoice.CustomerType,
                            isEstimate = (invoice.IsEstimateInvoice ?? "E").ToUpper() == "E",
                            billAmount = invoice.BillAmount,
                            discountPerItem = invoice.DiscountPerItem,
                            discountPercentage = invoice.DiscountPercentage,
                            isInState = invoice.IsInState,
                            custGstin = invoice.CustGstin,
                            totalBillAmount = invoice.TotalBillAmount,
                            amountPaid = invoice.AmountPaid,
                            balanceAmount = invoice.BalanceAmount,
                            invoiceNo = invoice.InvoiceNo,
                            vehicleNo = invoice.VehicleNo ?? string.Empty,
                            ownerName = invoice.OwnerName ?? string.Empty,
                            eWayBillNo = invoice.WayBill ?? string.Empty,
                            pos = invoice.Pos,
                            stateCode = invoice.StateCode,
                            pincode = invoice.Pincode,
                            createdDate = invoice.CreatedDate
                        };

                        var itemsTemp = _context.FwInvoiceItems
                            .AsNoTracking()
                            .Where(it => it.BillId == billId.Value)
                            .Select(it => new
                            {
                                slNo = it.SlNo,
                                billId = it.BillId,
                                itemId = it.ItemId,
                                itemCode = it.Item.ItemCode,
                                itemName = it.Item.ItemName,
                                itemTypeId = it.ItemTypeId,
                                itemTypeName = it.ItemType.TypeName,
                                quantity = it.Quantity,
                                itemPrice = it.ItemCost,
                                totalCost = it.TotalCost,
                                cgst = it.Cgst,
                                sgst = it.Sgst,
                                igst = it.Igst,
                                changedPrice = it.ChangedPrice,
                                isPriceModified = it.IsPriceModified,
                                sellType = it.SellType,
                                sellQty = it.SellQty,
                                originalCost = it.OriginalCost
                            }).ToList();

                        // Resolve HSN codes via master items -> master HSN to avoid EF navigation pitfalls
                        var invoiceItemIds = itemsTemp.Select(i => i.itemId).Distinct().ToList();
                        var invItemsData = _context.FwMasterItems.AsNoTracking()
                            .Where(mi => invoiceItemIds.Contains(mi.ItemId))
                            .Select(mi => new { mi.ItemId, mi.Hsnid })
                            .ToList();
                        var invHsnIds = invItemsData.Select(i => i.Hsnid).Where(x => x.HasValue).Select(x => (long)x.Value).Distinct().ToList();
                        var invHsnMap = invHsnIds.Any()
                            ? _context.FwMasterHsns.AsNoTracking().Where(h => invHsnIds.Contains(h.Hsnid)).ToDictionary(h => h.Hsnid, h => h.Hsncode)
                            : new System.Collections.Generic.Dictionary<long, string>();
                        var itemToHsnLookup = invItemsData.ToDictionary(i => i.ItemId, i => i.Hsnid.HasValue && invHsnMap.ContainsKey((long)i.Hsnid.Value) ? invHsnMap[(long)i.Hsnid.Value] : string.Empty);

                        var items = itemsTemp.Select(it =>
                        {
                            var normalizedQuantity = it.sellQty.HasValue && it.sellQty.Value > 0 && it.sellType.HasValue && it.sellType.Value > 0
                                ? _quantityConversionHelper.ConvertToLowestLevel(it.itemId, it.sellType.Value, it.sellQty.Value, it.itemPrice).normalizedQty
                                : it.quantity;
                            return new
                            {
                            it.slNo,
                            it.billId,
                            it.itemId,
                            it.itemCode,
                            it.itemName,
                            it.itemTypeId,
                            it.itemTypeName,
                            detailed = ItemTypeDescriptionHelper.GetItemTypewiseDescription(_context, it.itemId, normalizedQuantity),
                            originalEntry = it.sellQty.HasValue && it.sellQty.Value > 0
                                ? $"{it.sellQty.Value} {it.itemTypeName}'s"
                                : string.Empty,
                            quantity = normalizedQuantity,
                            it.itemPrice,
                            it.totalCost,
                            it.cgst,
                            it.sgst,
                            it.igst,
                            it.changedPrice,
                            it.isPriceModified,
                            it.sellType,
                            it.sellQty,
                            hsnCode = itemToHsnLookup.ContainsKey(it.itemId) ? itemToHsnLookup[it.itemId] : string.Empty,
                            it.originalCost
                            };
                        }).ToList().OrderByDescending(i => i.slNo);
                        // Recompute totals from items to ensure estimate PDF matches client-side amounts
                        var recomputedTotal = items.Sum(i => Convert.ToDecimal(i.totalCost));
                        // Create a header copy with recomputed total (keep nullable shape)
                        var header2 = new
                        {
                            billId = header.billId,
                            billNo = header.billNo,
                            customerId = header.customerId,
                            name = header.name,
                            custAddress = header.custAddress,
                            mobileNo = header.mobileNo,
                            mobile2 = header.mobile2,
                            customerType = header.customerType,
                            isEstimate = header.isEstimate,
                            billAmount = header.billAmount,
                            discountPerItem = header.discountPerItem,
                            discountPercentage = header.discountPercentage,
                            isInState = header.isInState,
                            custGstin = header.custGstin,
                            totalBillAmount = (decimal?)Math.Round(recomputedTotal, 2),
                            amountPaid = header.amountPaid,
                            balanceAmount = header.balanceAmount,
                            invoiceNo = header.invoiceNo,
                            pos = header.pos,
                            stateCode = header.stateCode,
                            pincode = header.pincode,
                            createdDate = header.createdDate,
                            eWayBillNo = header.eWayBillNo ?? string.Empty,
                            vehicleNo = header.vehicleNo ?? string.Empty,
                            ownerName = header.ownerName ?? string.Empty
                        };

                        var payload = new { header = header2, items };
                        ViewBag.EditJson = System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
                    }
                    else
                    {
                        var bill = _context.FwBills.AsNoTracking().FirstOrDefault(b => b.BillId == billId.Value);
                        if (bill != null)
                        {
                            var header = new
                            {
                                billId = bill.BillId,
                                billNo = bill.BillNo,
                                customerId = bill.CustomerId,
                                name = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == bill.CustomerId).Select(c => c.Name).FirstOrDefault(),
                                custAddress = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == bill.CustomerId).Select(c => c.CustAddress).FirstOrDefault(),
                                mobileNo = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == bill.CustomerId).Select(c => c.MobileNo).FirstOrDefault(),
                                mobile2 = _context.FwMasterCustomers.AsNoTracking().Where(c => c.CustomerId == bill.CustomerId).Select(c => c.Mobile2).FirstOrDefault(),
                                customerType = bill.CustomerType,
                                isEstimate = (bill.IsEstimateInvoice ?? "E").ToUpper() == "E",
                                billAmount = bill.BillAmount,
                                discountPerItem = bill.DiscountPerItem,
                                discountPercentage = bill.DiscountPercentage,
                                isInState = bill.IsInState,
                                custGstin = bill.CustGstin,
                                totalBillAmount = bill.TotalBillAmount,
                                amountPaid = bill.AmountPaid,
                                balanceAmount = bill.BalanceAmount,
                                invoiceNo = (string?)null,
                                pos = (string?)null,
                                stateCode = (string?)null,
                                pincode = (string?)null,
                                createdDate = bill.CreatedDate
                            };

                            var itemsTemp = _context.FwBillsItems
                                .AsNoTracking()
                                .Where(it => it.BillId == billId.Value)
                                .Select(it => new
                                {
                                    slNo = it.SlNo,
                                    billId = it.BillId,
                                    itemId = it.ItemId,
                                    itemCode = it.Item.ItemCode,
                                    itemName = it.Item.ItemName,
                                    itemTypeId = it.ItemTypeId,
                                    itemTypeName = it.ItemType.TypeName,
                                    quantity = it.Quantity,
                                    itemPrice = it.ItemCost,
                                    totalCost = it.TotalCost,
                                    cgst = it.Cgst,
                                    sgst = it.Sgst,
                                    igst = it.Igst,
                                    changedPrice = it.ChangedPrice,
                                    isPriceModified = it.IsPriceModified,
                                    sellType = it.SellType,
                                    sellQty = it.SellQty,
                                    hsnCode = it.Item.Hsnid,
                                    originalCost = it.OriginalCost
                                }).ToList();

                            var items = itemsTemp.Select(it =>
                            {
                                var normalizedQuantity = it.sellQty.HasValue && it.sellQty.Value > 0 && it.sellType.HasValue && it.sellType.Value > 0
                                    ? _quantityConversionHelper.ConvertToLowestLevel(it.itemId, it.sellType.Value, it.sellQty.Value, it.itemPrice).normalizedQty
                                    : it.quantity;
                                return new
                                {
                                it.slNo,
                                it.billId,
                                it.itemId,
                                it.itemCode,
                                it.itemName,
                                it.itemTypeId,
                                it.itemTypeName,
                                detailed = ItemTypeDescriptionHelper.GetItemTypewiseDescription(_context, it.itemId, normalizedQuantity),
                                originalEntry = it.sellQty.HasValue && it.sellQty.Value > 0
                                    ? $"{it.sellQty.Value} {it.itemTypeName}'s"
                                    : string.Empty,
                                quantity = normalizedQuantity,
                                it.itemPrice,
                                it.totalCost,
                                it.cgst,
                                it.sgst,
                                it.igst,
                                it.changedPrice,
                                it.isPriceModified,
                                it.sellType,
                                it.sellQty,
                                it.hsnCode,
                                it.originalCost
                                };
                            }).ToList().OrderByDescending(i => i.slNo);

                            var payload = new { header, items };
                            ViewBag.EditJson = System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading bill for edit: {ex.Message}");
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FireWorksBilling([FromBody] WebApp.Models.Dto.FireWorksBillingRequest request)
        {
            if (request == null)
                return Json(new { success = false, message = "Invalid request" });
            // Persist billing using EF Core into FW_Bills/FW_Bills_Items (estimates)
            // or FW_Invoice/FW_Invoice_Items (invoices). Support create and edit (request.BillId)
            try
            {
                var currentUserIdClaim = User.FindFirst("UserId")?.Value;
                var currentUserId = currentUserIdClaim != null ? int.Parse(currentUserIdClaim) : 0;

                // Basic server-side validation for numeric-only fields
                var validationErrors = new List<string>();

                // Mobile: must be numeric and positive when provided
                if (request.MobileNo.HasValue && request.MobileNo.Value <= 0)
                {
                    validationErrors.Add("Mobile number must be a positive numeric value.");
                }

                // Decimal fields: discount per item, discount percentage, amount paid must be non-negative and at most 2 decimal places
                bool HasMoreThanTwoDecimalPlaces(decimal? d) => d.HasValue && Decimal.Round(d.Value, 2) != d.Value;
                void CheckDecimalField(decimal? val, string fieldName)
                {
                    if (val.HasValue)
                    {
                        if (val.Value < 0)
                            validationErrors.Add($"{fieldName} must be non-negative.");
                        else if (HasMoreThanTwoDecimalPlaces(val))
                            validationErrors.Add($"{fieldName} must not have more than 2 decimal places.");
                    }
                }

                CheckDecimalField(request.DiscountPerItem, "DiscountPerItem");
                CheckDecimalField(request.DiscountPercentage, "DiscountPercentage");
                CheckDecimalField(request.AmountPaid, "AmountPaid");

                if (request.DiscountPerItem.HasValue && request.DiscountPerItem.Value > 100m)
                    validationErrors.Add("DiscountPerItem must be between 0 and 100.");
                if (request.DiscountPercentage.HasValue && request.DiscountPercentage.Value > 100m)
                    validationErrors.Add("DiscountPercentage must be between 0 and 100.");

                // Validate each item: quantity numeric and positive; price-related fields decimals
                if (request.Items != null)
                {
                    for (int i = 0; i < request.Items.Count; i++)
                    {
                        var itm = request.Items[i];
                        if (itm == null) continue;

                        if (itm.Quantity <= 0)
                        {
                            validationErrors.Add($"Item at index {i} has invalid Quantity. Quantity must be a positive integer.");
                        }

                        // Price fields
                        CheckDecimalField(itm.ItemPrice, $"Items[{i}].ItemPrice");
                        var requestedItemCost = itm.ItemCost ?? itm.ItemPrice;
                        if (!requestedItemCost.HasValue || requestedItemCost.Value <= 0m)
                            validationErrors.Add($"Items[{i}].ItemCost must be greater than zero.");
                        else
                            CheckDecimalField(requestedItemCost, $"Items[{i}].ItemCost");
                        CheckDecimalField(itm.ChangedPrice, $"Items[{i}].ChangedPrice");
                        CheckDecimalField(itm.OriginalCost, $"Items[{i}].OriginalCost");
                        CheckDecimalField(itm.CgstAmount, $"Items[{i}].CgstAmount");
                        CheckDecimalField(itm.SgstAmount, $"Items[{i}].SgstAmount");
                        CheckDecimalField(itm.IgstAmount, $"Items[{i}].IgstAmount");
                        CheckDecimalField(itm.CgstPercent, $"Items[{i}].CgstPercent");
                        CheckDecimalField(itm.SgstPercent, $"Items[{i}].SgstPercent");
                        CheckDecimalField(itm.IgstPercent, $"Items[{i}].IgstPercent");
                        if (itm.CgstPercent is < 0m or > 100m)
                            validationErrors.Add($"Items[{i}].CgstPercent must be between 0 and 100.");
                        if (itm.SgstPercent is < 0m or > 100m)
                            validationErrors.Add($"Items[{i}].SgstPercent must be between 0 and 100.");
                        if (itm.IgstPercent is < 0m or > 100m)
                            validationErrors.Add($"Items[{i}].IgstPercent must be between 0 and 100.");
                    }
                }
                if (request.Items == null || request.Items.Count == 0 ||
                    request.Items.Any(it => it == null || it.ItemId <= 0 || it.Quantity <= 0))
                {
                    validationErrors.Add("At least one valid item with a positive quantity is required.");
                }

                // FW_Bills_Items and FW_Invoice_Items identify a line by item. Reject
                // duplicate item identities up front instead of allowing one line to
                // overwrite another or fail later with a database key violation.
                if (request.Items != null)
                {
                    var duplicateLine = request.Items
                        .Where(it => it != null && it.ItemId > 0 && it.Quantity > 0)
                        .GroupBy(it => it.ItemId)
                        .FirstOrDefault(g => g.Count() > 1);
                    if (duplicateLine != null)
                        validationErrors.Add($"Item {duplicateLine.Key} appears more than once.");
                }

                if (validationErrors.Any())
                {
                    return Json(new { success = false, message = "Validation failed", errors = validationErrors });
                }

                // Use Serializable isolation to safely increment master bill number without stored procedure
                using (var tx = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    // Financial validation tolerance (currency units). If client values deviate more than this, reject save.
                    const decimal financialTolerance = 0.01m;
                    // Resolve or create customer similar to USP_DML_Bills behavior
                    int resolvedCustomerId = 0;
                    try
                    {
                        if (request.CustomerId.HasValue && request.CustomerId.Value > 0)
                        {
                            resolvedCustomerId = request.CustomerId.Value;
                        }
                        else if (request.MobileNo.HasValue && request.MobileNo.Value > 0)
                        {
                            var existCust = _context.FwMasterCustomers.FirstOrDefault(c => c.MobileNo == request.MobileNo.Value);
                            if (existCust != null)
                            {
                                // update details
                                existCust.Name = request.Name ?? existCust.Name;
                                existCust.CustomerType = (byte)request.CustomerType;
                                existCust.CustAddress = request.Address ?? existCust.CustAddress;
                                if (request.Mobile2.HasValue && request.Mobile2.Value > 0)
                                    existCust.Mobile2 = request.Mobile2;
                                else if (!existCust.Mobile2.HasValue && request.MobileNo.HasValue && request.MobileNo.Value > 0)
                                    existCust.Mobile2 = request.MobileNo;
                                // update customer GSTIN if provided
                                try { if (!string.IsNullOrWhiteSpace(request.CustomerGstin)) existCust.GetType().GetProperty("CustGstin")?.SetValue(existCust, request.CustomerGstin); } catch { }
                                _context.SaveChanges();
                                resolvedCustomerId = (int)existCust.CustomerId;
                            }
                            else
                            {
                                var newCust = new FwMasterCustomer
                                {
                                    Name = request.Name ?? string.Empty,
                                    MobileNo = request.MobileNo.Value,
                                    CustomerType = (byte)request.CustomerType,
                                    CustAddress = request.Address ?? string.Empty,
                                    // set CustGstin if provided on request
                                    Mobile2 = request.Mobile2.HasValue && request.Mobile2.Value > 0 ? request.Mobile2 : request.MobileNo,
                                    CreatedBy = currentUserId,
                                    CreatedDate = DateTime.Now
                                };
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(request.CustomerGstin))
                                        newCust.GetType().GetProperty("CustGstin")?.SetValue(newCust, request.CustomerGstin);
                                }
                                catch { }
                                _context.FwMasterCustomers.Add(newCust);
                                _context.SaveChanges();
                                resolvedCustomerId = (int)newCust.CustomerId;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error resolving customer: {ex.Message}");
                        // proceed with resolvedCustomerId = 0
                    }
                    if (request.IsEstimate)
                    {
                        // FW_Bills (estimates)
                        bool isEdit = request.BillId.HasValue && request.BillId.Value > 0;
                        FwBill billEntity;
                        // preservedPackedKeys tracks packed rows that should keep their IsPacked flag when re-adding items
                        var preservedPackedKeys = new System.Collections.Generic.HashSet<string>();
                        if (isEdit)
                        {
                            // If this bill has already been converted to an invoice, disallow editing the estimate
                            var existingInvoiceForBill = _context.FwInvoices.AsNoTracking().FirstOrDefault(i => i.BillId == request.BillId.Value);
                            if (existingInvoiceForBill != null)
                            {
                                tx.Rollback();
                                return Json(new { success = false, message = "Estimate has been converted to an invoice and cannot be modified." });
                            }
                            billEntity = _context.FwBills.Include(b => b.FwBillsItems).FirstOrDefault(b => b.BillId == request.BillId.Value);
                            if (billEntity == null)
                            {
                                tx.Rollback();
                                return Json(new { success = false, message = "Estimate not found for edit" });
                            }

                            // remove existing items - we'll re-add

                            if (billEntity.FwBillsItems != null && billEntity.FwBillsItems.Any())
                            {
                                // Prevent deletion/modification of packed items: if any existing item is packed and not present unchanged in request, reject
                                var packedExisting = billEntity.FwBillsItems.Where(x => x.IsPacked == true).ToList();
                                if (packedExisting.Any())
                                {
                                    // Only warn/reject when Quantity or Price (ItemCost) changes for packed items.
                                    // Build a set of preserved packed keys (ItemId:ItemTypeId:Quantity:ItemCost) for which there is
                                    // a matching request row with identical Quantity and computed ItemCost. Any packed item that
                                    // is missing or has different Quantity/ItemCost will cause a rejection.
                                    preservedPackedKeys.Clear();
                                    var requestItems = (request.Items ?? new System.Collections.Generic.List<WebApp.Models.Dto.FireWorksBillingItemDto>()).Where(r => r != null).ToList();

                                    foreach (var p in packedExisting)
                                    {
                                        // find matching request rows by ItemId and ItemTypeId
                                        var matches = requestItems.Where(rit => rit.ItemId == p.ItemId && (rit.ItemTypeId ?? 0) == p.ItemTypeId).ToList();
                                        if (!matches.Any())
                                        {
                                            // item removed entirely => treated as quantity change => reject
                                            string? itemName = null;
                                            try { itemName = p.Item?.ItemName ?? _context.FwMasterItems.AsNoTracking().FirstOrDefault(m => m.ItemId == p.ItemId)?.ItemName; } catch { }
                                            var msg = !string.IsNullOrWhiteSpace(itemName) ? $"Cannot modify or delete packed item: {itemName}." : $"Cannot modify or delete packed item (ItemId: {p.ItemId}).";
                                            tx.Rollback();
                                            return Json(new { success = false, message = msg });
                                        }

                                        // Check if any matching request row preserves Quantity and ItemCost (price)
                                        var matchedPreserve = false;
                                        foreach (var rit in matches)
                                        {
                                            // compute request item cost using same rules as persistence: ChangedPrice if provided, otherwise OriginalCost or ItemPrice
                                            decimal? changedPrice = rit.ChangedPrice != null ? rit.ChangedPrice : null;
                                            decimal originalCost = rit.OriginalCost ?? (rit.ItemPrice ?? 0m);
                                            bool isPriceModified = changedPrice.HasValue;
                                            decimal reqItemCost = isPriceModified ? changedPrice.Value : originalCost;

                                            // Compare quantity and price (rounded to 2 decimals) to tolerate minor differences
                                            if (rit.Quantity == p.Quantity && Decimal.Round(reqItemCost, 2) == Decimal.Round(p.ItemCost, 2))
                                            {
                                                var key = $"{p.ItemId}:{p.ItemTypeId}:{p.Quantity}:{Decimal.Round(p.ItemCost, 2)}";
                                                preservedPackedKeys.Add(key);
                                                matchedPreserve = true;
                                                break;
                                            }
                                        }

                                        if (!matchedPreserve)
                                        {
                                            // Quantity or price changed for this packed item => reject
                                            string? itemName = null;
                                            try { itemName = p.Item?.ItemName ?? _context.FwMasterItems.AsNoTracking().FirstOrDefault(m => m.ItemId == p.ItemId)?.ItemName; } catch { }
                                            var msg = !string.IsNullOrWhiteSpace(itemName) ? $"Cannot modify Quantity or Price of packed item: {itemName}." : $"Cannot modify Quantity or Price of packed item (ItemId: {p.ItemId}).";
                                            tx.Rollback();
                                            return Json(new { success = false, message = msg });
                                        }
                                    }
                                }

                            }

                            billEntity.ModifiedBy = currentUserId;
                            billEntity.ModifiedDate = DateTime.Now;
                            billEntity.CustomerType = (byte)request.CustomerType;
                            billEntity.IsEstimateInvoice = request.IsEstimate ? "E" : "I";
                            // Update discount and state fields on edit
                            billEntity.DiscountPerItem = request.DiscountPerItem;
                            billEntity.DiscountPercentage = request.DiscountPercentage;
                            billEntity.IsInState = request.IsInState;
                            //billEntity.BillNo = request.InvoiceNo;
                        }
                        else
                        {
                            // generate BillNo using shared BaseController helper
                            var (generatedBillNo, seq) = GenerateDocumentNumber(_context, request.IsEstimate);

                            billEntity = new FwBill
                            {
                                CustomerId = resolvedCustomerId,
                                CustomerType = (byte)request.CustomerType,
                                IsEstimateInvoice = request.IsEstimate ? "E" : "I",
                                CreatedBy = currentUserId,
                                CreatedDate = DateTime.Now,
                                BillNo = string.IsNullOrWhiteSpace(request.InvoiceNo) ? generatedBillNo : request.InvoiceNo,
                                DiscountPerItem = request.DiscountPerItem,
                                DiscountPercentage = request.DiscountPercentage,
                                IsInState = request.IsInState,
                                // initialize state as in stored proc
                                IsVerified = false,
                                DispatchStatus = "I"
                            };
                            _context.FwBills.Add(billEntity);
                            _context.SaveChanges(); // obtain BillId
                        }

                        decimal billAmount = 0m;
                        var perRowValidationErrors = new System.Collections.Generic.List<string>();
                        var updatedExistingItems = new HashSet<FwBillsItem>();

                        foreach (var it in request.Items ?? new System.Collections.Generic.List<WebApp.Models.Dto.FireWorksBillingItemDto>())
                        {
                            if (it == null || it.ItemId <= 0 || it.Quantity <= 0)
                                continue;

                            // Preserve price modification state from the client; retain fallback behavior for older clients.
                            decimal suppliedItemCost = it.ItemCost ?? it.ItemPrice ?? 0m;
                            decimal originalCost = it.OriginalCost ?? suppliedItemCost;
                            bool isPriceModified = it.IsPriceModified ?? it.ChangedPrice.HasValue;
                            decimal? changedPrice = isPriceModified
                                ? (it.ChangedPrice ?? suppliedItemCost)
                                : null;

                            // ItemCost is the lowest-level rate supplied by the grid; only an explicit price edit overrides it.
                            decimal itemCost = isPriceModified ? changedPrice.GetValueOrDefault() : suppliedItemCost;

                            // The billing grid sends Quantity in lowest-level units.
                            // SellQty/SellType are retained as display metadata and must
                            // not convert the already-normalized quantity a second time.
                            var normalizedQty = it.Quantity;
                            var totalCost = Math.Round(normalizedQty * itemCost, 2, MidpointRounding.AwayFromZero);

                            if (it.TotalCost > 0m && Math.Abs(it.TotalCost - totalCost) > financialTolerance)
                            {
                                perRowValidationErrors.Add($"Item {it.ItemId} total does not match its quantity and price.");
                            }

                            billAmount += totalCost;

                            // Determine whether this row should preserve its packed status (quantity and price unchanged)
                            var newItemKey = $"{it.ItemId}:{it.ItemTypeId}:{it.Quantity}:{Decimal.Round(itemCost, 2)}";
                            var itemEntity = isEdit
                                ? billEntity.FwBillsItems.FirstOrDefault(x => x.ItemId == it.ItemId && x.ItemTypeId == (it.ItemTypeId ?? 0))
                                : null;
                            var isExistingItem = itemEntity != null;
                            itemEntity ??= new FwBillsItem { BillId = billEntity.BillId };
                            itemEntity.ItemId = it.ItemId;
                            itemEntity.ItemTypeId = it.ItemTypeId ?? 0;
                            itemEntity.Quantity = normalizedQty;
                            itemEntity.SellType = it.SellType;
                            itemEntity.SellQty = it.SellQty;
                            itemEntity.OriginalCost = originalCost;
                            itemEntity.ChangedPrice = changedPrice;
                            itemEntity.IsPriceModified = isPriceModified;
                            itemEntity.Cgst = it.CgstPercent != 0m ? it.CgstPercent : null;
                            itemEntity.Sgst = it.SgstPercent != 0m ? it.SgstPercent : null;
                            itemEntity.Igst = it.IgstPercent != 0m ? it.IgstPercent : null;
                            itemEntity.SlNo = it.SlNo;
                            if (!isExistingItem)
                                itemEntity.IsPacked = preservedPackedKeys.Contains(newItemKey);
                            itemEntity.ItemCost = itemCost;
                            itemEntity.TotalCost = totalCost;
                            if (isExistingItem)
                            {
                                updatedExistingItems.Add(itemEntity);
                                _context.Entry(itemEntity).State = EntityState.Modified;
                            }
                            else
                            {
                                _context.Entry(itemEntity).State = EntityState.Added;
                                _context.FwBillsItems.Add(itemEntity);
                            }
                        }

                        if (perRowValidationErrors.Any())
                        {
                            tx.Rollback();
                            return Json(new { success = false, message = "Validation failed", errors = perRowValidationErrors });
                        }

                        if (isEdit)
                        {
                            var staleItems = billEntity.FwBillsItems
                                .Where(x => x.IsPacked != true && !updatedExistingItems.Contains(x))
                                .ToList();
                            if (staleItems.Count > 0)
                                _context.FwBillsItems.RemoveRange(staleItems);
                        }

                        billEntity.BillAmount = billAmount;
                        billEntity.TotalBillAmount = billAmount;
                        billEntity.FinalAmount = billAmount;
                        // set payment fields if provided
                        var estimateAmountPaid = Math.Max(0m, request.AmountPaid ?? 0m);
                        billEntity.AmountPaid = Math.Round(Math.Min(estimateAmountPaid, billAmount), 2, MidpointRounding.AwayFromZero);
                        billEntity.BalanceAmount = Math.Round(billAmount - billEntity.AmountPaid.Value, 2, MidpointRounding.AwayFromZero);

                        await _context.SaveChangesAsync();

                        var persistedItems = _context.FwBillsItems
                            .AsNoTracking()
                            .Where(x => x.BillId == billEntity.BillId)
                            .ToList();
                        var expectedItemCount = request.Items?.Count(x => x != null && x.ItemId > 0 && x.Quantity > 0) ?? 0;
                        if (persistedItems.Count != expectedItemCount)
                        {
                            throw new InvalidOperationException($"Persisted bill item count failed validation (expected {expectedItemCount}, found {persistedItems.Count}).");
                        }

                        const decimal persistedCostTolerance = 0.01m;
                        var persistedCostMismatch = persistedItems.FirstOrDefault(x =>
                            x.ItemCost < 0m ||
                            Math.Abs(x.TotalCost - Math.Round(x.Quantity * x.ItemCost, 2, MidpointRounding.AwayFromZero)) > persistedCostTolerance);
                        if (persistedCostMismatch != null)
                        {
                            var expectedTotal = Math.Round(persistedCostMismatch.Quantity * persistedCostMismatch.ItemCost, 2, MidpointRounding.AwayFromZero);
                            throw new InvalidOperationException(
                                $"Persisted bill item cost failed validation for item {persistedCostMismatch.ItemId} " +
                                $"(quantity {persistedCostMismatch.Quantity}, cost {persistedCostMismatch.ItemCost:0.00}, " +
                                $"stored total {persistedCostMismatch.TotalCost:0.00}, expected {expectedTotal:0.00}).");
                        }
                        tx.Commit();

                        // After save behavior:
                        // - Always provide previewUrl so client opens PDF in a new tab.
                        // - If this is a new bill (not edit): do not provide redirectUrl (save-only, client stays on form).
                        // - If this is an edit: provide redirectUrl so client navigates to ViewBills after opening preview.
                        var previewEstimateUrl = Url.Action("PreviewPdf", "Billing", new { billId = billEntity.BillId, template = "Estimate" }, Request.Scheme, Request.Host.Value);
                        if (!isEdit && request.IsEstimate)
                        {
                            // New estimate created: return preview only, no redirect
                            return Json(new { success = true, message = "Saved", billId = billEntity.BillId, previewUrl = previewEstimateUrl });
                        }

                        if (isEdit)
                        {
                            var redirect = Url.Action("ViewBills", "Home");
                            return Json(new { success = true, message = "Saved", billId = billEntity.BillId, previewUrl = previewEstimateUrl, redirectUrl = redirect });
                        }

                        return Json(new { success = true, message = "Saved", billId = billEntity.BillId, previewUrl = previewEstimateUrl });
                    }
                    else
                    {
                        // Invoice path - use FW_Invoice and FW_Invoice_Items

                        // Enforce business rule: invoices must be created only against an existing estimate (Bill).
                        if (!request.BillId.HasValue || request.BillId.Value <= 0)
                        {
                            return Json(new { success = false, message = "Cannot create invoice without an existing estimate. Please save an estimate first or select an existing estimate before generating an invoice." });
                        }

                        // Additionally validate that the referenced bill exists
                        var linkedBill = _context.FwBills.AsNoTracking().FirstOrDefault(b => b.BillId == request.BillId.Value);
                        if (linkedBill == null)
                        {
                            return Json(new { success = false, message = "Referenced estimate (Bill) not found. Cannot generate invoice without a valid estimate." });
                        }

                        var billItems = _context.FwBillsItems
                            .AsNoTracking()
                            .Where(item => item.BillId == request.BillId.Value)
                            .ToList();
                        if (billItems.Count == 0 || billItems.Any(item => item.IsPacked != true))
                        {
                            return Json(new { success = false, message = "All items must be dispatched/loaded before converting the estimate to an invoice." });
                        }

                        // Determine whether an invoice already exists for this bill before making changes.
                        var existingInvoiceForBill = _context.FwInvoices.AsNoTracking().FirstOrDefault(i => i.BillId == request.BillId.Value);
                        bool invoiceExisted = existingInvoiceForBill != null;

                        FwInvoice invEntity = null;
                        if (invoiceExisted)
                        {
                            // Load the existing invoice for update
                            invEntity = _context.FwInvoices.FirstOrDefault(i => i.BillId == request.BillId.Value);
                        }
                        else
                        {
                            // No invoice exists yet for this bill - create a new invoice record linked to the bill
                            var (_, seq) = GenerateDocumentNumber(_context, request.IsEstimate);
                            var generatedInvoiceNo = "E" + seq.ToString("D4");
                            // InvoiceId is a manually generated legacy key. Count()+1 can
                            // reuse an id after a deletion, so derive it from the current
                            // maximum while the serializable transaction is active.
                            var newInvoiceId = (_context.FwInvoices
                                .Select(i => (long?)i.InvoiceId)
                                .Max() ?? 0L) + 1L;

                            int linkedBillId = request.BillId ?? 0;
                            string billNoToUse = request.InvoiceNo;
                            int invoiceCustomerId = resolvedCustomerId;
                            byte invoiceCustomerType = (byte)request.CustomerType;
                            if (linkedBillId > 0)
                            {
                                var billRef = _context.FwBills.AsNoTracking().FirstOrDefault(b => b.BillId == linkedBillId);
                                if (billRef != null)
                                {
                                    billNoToUse = !string.IsNullOrWhiteSpace(billRef.BillNo) ? billRef.BillNo : billNoToUse;
                                    invoiceCustomerId = billRef.CustomerId;
                                    invoiceCustomerType = billRef.CustomerType;
                                }
                            }

                            invEntity = new FwInvoice
                            {
                                InvoiceId = newInvoiceId,
                                BillId = linkedBillId,
                                BillNo = billNoToUse,
                                CustomerId = invoiceCustomerId,
                                CustomerType = invoiceCustomerType,
                                IsEstimateInvoice = request.IsEstimate ? "E" : "I",
                                CreatedBy = currentUserId,
                                CreatedDate = DateTime.Now,
                                InvoiceNo = generatedInvoiceNo,
                                WayBill = request.EWayBillNo?.Trim(),
                                VehicleNo = request.VehicleNo?.Trim(),
                                OwnerName = request.OwnerName?.Trim(),
                                DiscountPerItem = request.DiscountPerItem,
                                DiscountPercentage = request.DiscountPercentage,
                                IsInState = request.IsInState,
                                AmountPaid = request.AmountPaid ?? 0m,
                                BalanceAmount = request.BalanceAmount ?? 0m,
                                Invoice = request.Invoice ?? string.Empty,
                                Pos = request.POS ?? string.Empty,
                                StateCode = request.StateCode ?? string.Empty,
                                Pincode = request.Pincode ?? string.Empty,
                                // persist customer GSTIN if provided
                                CustGstin = request.CustomerGstin ?? string.Empty,
                                Loc = request.Loc ?? string.Empty
                            };
                            _context.FwInvoices.Add(invEntity);
                            _context.SaveChanges();
                        }

                        // remove existing items for this invoice (if any) so we'll re-add
                        var existingItems = _context.FwInvoiceItems.Where(x => x.InvoiceId == invEntity.InvoiceId).ToList();
                        if (existingItems.Any())
                        {
                            _context.FwInvoiceItems.RemoveRange(existingItems);
                            _context.SaveChanges();
                        }

                        // Ensure invoice customer and bill info remain consistent with the originating bill when present
                        if (request.BillId.HasValue && request.BillId.Value > 0)
                        {
                            var billRefForUpdate = _context.FwBills.AsNoTracking().FirstOrDefault(b => b.BillId == request.BillId.Value);
                            if (billRefForUpdate != null)
                            {
                                invEntity.CustomerId = billRefForUpdate.CustomerId;
                                invEntity.CustomerType = billRefForUpdate.CustomerType;
                                invEntity.BillNo = billRefForUpdate.BillNo ?? invEntity.BillNo;
                            }
                            else
                            {
                                invEntity.CustomerType = (byte)request.CustomerType;
                            }
                        }
                        else
                        {
                            invEntity.CustomerType = (byte)request.CustomerType;
                        }
                        invEntity.ModifiedBy = currentUserId;
                        invEntity.ModifiedDate = DateTime.Now;
                        invEntity.IsEstimateInvoice = request.IsEstimate ? "E" : "I";
                        // Only overwrite InvoiceNo when a non-empty value is provided by client
                        if (!string.IsNullOrWhiteSpace(request.InvoiceNo))
                            invEntity.InvoiceNo = request.InvoiceNo;
                        // Invoice header fields
                        invEntity.Invoice = request.Invoice ?? invEntity.Invoice;
                        invEntity.Pos = request.POS ?? invEntity.Pos;
                        invEntity.StateCode = request.StateCode ?? invEntity.StateCode;
                        invEntity.Pincode = request.Pincode ?? invEntity.Pincode;
                        invEntity.Loc = request.Loc ?? invEntity.Loc;
                        invEntity.WayBill = request.EWayBillNo?.Trim() ?? invEntity.WayBill;
                        invEntity.VehicleNo = request.VehicleNo?.Trim() ?? invEntity.VehicleNo;
                        invEntity.OwnerName = request.OwnerName?.Trim() ?? invEntity.OwnerName;
                        // Update discount/state/payment fields when editing invoice
                        invEntity.DiscountPerItem = request.DiscountPerItem;
                        invEntity.DiscountPercentage = request.DiscountPercentage;
                        invEntity.IsInState = request.IsInState;
                        invEntity.AmountPaid = request.AmountPaid ?? invEntity.AmountPaid;
                        invEntity.BalanceAmount = request.BalanceAmount ?? invEntity.BalanceAmount;
                        invEntity.CustGstin = request.CustomerGstin ?? invEntity.CustGstin;

                        decimal invAmount = 0m;
                        decimal invCgstAmount = 0m;
                        decimal invSgstAmount = 0m;
                        decimal invIgstAmount = 0m;
                        int sl = 1;

                        // build conversion map for items present in request so qtyPerType can be resolved when not supplied
                        var reqItemIds = (request.Items ?? new System.Collections.Generic.List<WebApp.Models.Dto.FireWorksBillingItemDto>())
                            .Where(x => x != null && x.ItemId > 0).Select(x => x.ItemId).Distinct().ToList();
                        var conversionRows = _context.FwMasterItemsConversions.AsNoTracking()
                            .Where(c => reqItemIds.Contains(c.ItemId))
                            .Select(c => new { c.ItemId, c.ItemTypeId, c.QtyPerType })
                            .ToList();

                        var convMap = conversionRows.GroupBy(c => c.ItemId)
                            .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.ItemTypeId, x => x.QtyPerType));

                        // invoice-level discount fallback: prefer request.DiscountPerItem (percent 0-100), else 0
                        var headerDiscountPercent = request.DiscountPerItem.HasValue && request.DiscountPerItem.Value != 0m
                            ? request.DiscountPerItem.Value
                            : 0m;
                        if (headerDiscountPercent < 0m) headerDiscountPercent = 0m;
                        if (headerDiscountPercent > 100m) headerDiscountPercent = 100m;

                        foreach (var it in request.Items ?? new System.Collections.Generic.List<WebApp.Models.Dto.FireWorksBillingItemDto>())
                        {
                            if (it == null || it.ItemId <= 0 || it.Quantity <= 0)
                                continue;

                            decimal originalCost = it.OriginalCost ?? (it.ItemPrice ?? 0m);
                            bool isPriceModified = it.IsPriceModified ?? it.ChangedPrice.HasValue;
                            decimal? changedPrice = isPriceModified
                                ? (it.ChangedPrice ?? it.ItemPrice ?? originalCost)
                                : null;
                            decimal unitRatePerBase = isPriceModified ? changedPrice.GetValueOrDefault() : originalCost;

                            // base units (stored) and qtyPerType (from payload or conversions)
                            var baseQty = it.Quantity;
                            int qtyPerType = 1;
                            if (it.QtyPerType.HasValue && it.QtyPerType.Value > 0)
                                qtyPerType = it.QtyPerType.Value;
                            else
                            {
                                try
                                {
                                    if (convMap.ContainsKey(it.ItemId) && convMap[it.ItemId].ContainsKey(it.ItemTypeId ?? 0))
                                        qtyPerType = convMap[it.ItemId][it.ItemTypeId ?? 0] ?? 1;
                                }
                                catch { qtyPerType = 1; }
                            }
                            if (qtyPerType <= 0) qtyPerType = 1;

                            var displayQty = (decimal)baseQty / (decimal)qtyPerType;

                            // compute rate per displayed type-unit and apply header discount as percentage
                            var ratePerType = unitRatePerBase * qtyPerType;
                            var discountedRatePerType = ratePerType * (1 - (headerDiscountPercent / 100m));
                            if (discountedRatePerType < 0m) discountedRatePerType = 0m;

                            var taxable = Math.Round(displayQty * discountedRatePerType, 2, MidpointRounding.AwayFromZero);

                            // Tax values in the request are rates. Calculate the amounts
                            // from the authoritative taxable value so persisted totals
                            // cannot be changed by a client supplied TotalCost.
                            var cgstPercent = Math.Clamp(it.CgstPercent ?? 0m, 0m, 100m);
                            var sgstPercent = Math.Clamp(it.SgstPercent ?? 0m, 0m, 100m);
                            var igstPercent = Math.Clamp(it.IgstPercent ?? 0m, 0m, 100m);
                            var cgstAmount = Math.Round(taxable * cgstPercent / 100m, 2, MidpointRounding.AwayFromZero);
                            var sgstAmount = Math.Round(taxable * sgstPercent / 100m, 2, MidpointRounding.AwayFromZero);
                            var igstAmount = Math.Round(taxable * igstPercent / 100m, 2, MidpointRounding.AwayFromZero);

                            invAmount += taxable;
                            invCgstAmount += cgstAmount;
                            invSgstAmount += sgstAmount;
                            invIgstAmount += igstAmount;

                            var invItem = new FwInvoiceItem
                            {
                                InvoiceId = invEntity.InvoiceId,
                                BillId = invEntity.BillId,
                                ItemId = it.ItemId,
                                ItemTypeId = it.ItemTypeId ?? 0,
                                Quantity = it.Quantity,
                                OriginalCost = originalCost,
                                ChangedPrice = changedPrice,
                                IsPriceModified = isPriceModified,
                                // store item cost per base unit for historical/reference
                                ItemCost = unitRatePerBase,
                                // store GST PERCENT values (prefer client-sent per-line percents; fall back to defaults)
                                Cgst = it.CgstPercent,
                                Sgst = it.SgstPercent,
                                Igst = it.IgstPercent,
                                TotalCost = taxable,
                                SlNo = sl++
                            };
                            _context.FwInvoiceItems.Add(invItem);
                        }

                        invAmount = Math.Round(invAmount, 2, MidpointRounding.AwayFromZero);
                        var taxAmount = Math.Round(invCgstAmount + invSgstAmount + invIgstAmount, 2, MidpointRounding.AwayFromZero);
                        var invoiceTotal = Math.Round(invAmount + taxAmount, 2, MidpointRounding.AwayFromZero);
                        invEntity.BillAmount = invAmount;
                        invEntity.TotalBillAmount = invoiceTotal;
                        var amountPaid = Math.Max(0m, invEntity.AmountPaid ?? 0m);
                        invEntity.AmountPaid = Math.Round(Math.Min(amountPaid, invoiceTotal), 2, MidpointRounding.AwayFromZero);
                        invEntity.BalanceAmount = Math.Round(invoiceTotal - invEntity.AmountPaid.Value, 2, MidpointRounding.AwayFromZero);

                        _context.SaveChanges();
                        tx.Commit();

                        // Return InvoiceId and provide preview + redirect URLs so client can open PDF in a new tab and navigate to ViewBills
                        var chosenTemplate = request.IsInState.HasValue && !request.IsInState.Value ? "InvoiceOutOfState" : "InvoiceInState";
                        var previewUrl = Url.Action("PreviewPdf", "Billing", new { billId = invEntity.BillId, template = chosenTemplate }, Request.Scheme, Request.Host.Value);
                        var redirect = Url.Action("ViewBills", "Home");
                        // If we created a new invoice record for this bill, do not redirect the current window; only provide previewUrl.
                        if (!invoiceExisted)
                        {
                            return Json(new { success = true, message = "Saved", billId = invEntity.InvoiceId, previewUrl = previewUrl });
                        }

                        // Existing invoice edited -> provide redirect so client navigates to ViewBills after opening preview.
                        return Json(new { success = true, message = "Saved", billId = invEntity.InvoiceId, previewUrl = previewUrl, redirectUrl = redirect });
                    }
                }
            }
            catch (System.Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.FireWorksBilling",
                    $"BillId={request?.BillId}, CustomerId={request?.CustomerId}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error saving fireworks billing: {ex.Message}");
                return Json(new { success = false, message = $"Error saving billing: {ex.Message}" });
            }
        }

        // POST: /Billing/StockTransfer
        [HttpPost]
        public async Task<IActionResult> StockTransfer([FromBody] WebApp.Models.Dto.StockTransferRequest request)
        {
            if (request == null)
                return Json(new { success = false, message = "Invalid request" });

            // Validate basic request
            if (request.Items == null || !request.Items.Any())
                return Json(new { success = false, message = "No items to transfer" });

            try
            {
                var currentUserIdClaim = User.FindFirst("UserId")?.Value;
                var currentUserId = currentUserIdClaim != null ? int.Parse(currentUserIdClaim) : 0;

                // Wrap the whole transfer (header, items, stock movements) in a single transaction
                using (var tx = _context.Database.BeginTransaction())
                {
                    try
                    {
                        FwStockTransfer transfer;
                        bool isEdit = request.TransferId.HasValue && request.TransferId.Value > 0;
                        if (isEdit)
                        {
                            transfer = _context.FwStockTransfers
                                .Include(t => t.Items)
                                .FirstOrDefault(t => t.TransferId == request.TransferId.Value);
                            if (transfer == null)
                            {
                                tx.Rollback();
                                return Json(new { success = false, message = "Transfer not found for edit" });
                            }

                            // update header
                            transfer.FromGodown = request.FromGodown;
                            transfer.ToGodown = request.ToGodown;
                            // remove existing items; we'll re-add
                            if (transfer.Items != null && transfer.Items.Any())
                            {
                                _context.FwStockTransferItems.RemoveRange(transfer.Items);
                            }
                            _context.SaveChanges();
                        }
                        else
                        {
                            transfer = new FwStockTransfer
                            {
                                FromGodown = request.FromGodown,
                                ToGodown = request.ToGodown,
                                CreatedBy = currentUserId,
                                CreatedDate = DateTime.Now
                            };
                            _context.FwStockTransfers.Add(transfer);
                            _context.SaveChanges(); // persist to obtain TransferId within transaction
                        }

                        // For each item, call stored procedure USP_GET_ItemLowestPrice using EF raw SQL and compute converted quantity
                        int addedItemCount = 0;
                        foreach (var item in request.Items)
                        {
                            // Validate incoming item: if ItemId missing/invalid or Quantity not positive, treat as cleared/skip
                            if (item == null || item.ItemId <= 0 || item.Quantity <= 0)
                            {
                                // Skip invalid or partially-entered items so old references are not preserved
                                continue;
                            }

                            // Ensure the item actually exists in master items and that the client-side name matches
                            var masterItem = _context.FwMasterItems.AsNoTracking().FirstOrDefault(m => m.ItemId == item.ItemId);
                            if (masterItem == null)
                            {
                                // unknown item id, skip to avoid preserving stale references
                                continue;
                            }

                            // If client provided an ItemName but it doesn't match master, treat as cleared/skip
                            if (!string.IsNullOrWhiteSpace(item.ItemName) && !string.Equals(item.ItemName.Trim(), masterItem.ItemName?.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                // user likely typed or changed the name without selecting — skip to avoid storing wrong id
                                continue;
                            }
                            var p1 = new SqlParameter("@ItemId", System.Data.SqlDbType.Int) { Value = item.ItemId };
                            var p2 = new SqlParameter("@ItemTypeId", System.Data.SqlDbType.Int) { Value = (object?)item.ItemTypeId ?? 0 };
                            var p3 = new SqlParameter("@CustomerType", System.Data.SqlDbType.Int) { Value = 1 };

                            var rows = _context.Set<ItemLowestPriceDto>()
                                .FromSqlRaw("EXEC USP_GET_ItemLowestPrice @ItemId, @ItemTypeId, @CustomerType", p1, p2, p3)
                                .AsNoTracking()
                                .ToList();

                            if (rows != null && rows.Count > 0)
                            {
                                var first = rows[0];
                                var lowestQty = item.Quantity * first.QtyPerType;
                                var transferItem = new FwStockTransferItem
                                {
                                    TransferId = transfer.TransferId,
                                    ItemId = item.ItemId,
                                    ItemTypeId = item.ItemTypeId ?? 0,
                                    Quantity = item.Quantity,
                                    QtyLowest = lowestQty
                                };
                                _context.FwStockTransferItems.Add(transferItem);
                                addedItemCount++;
                            }
                            else
                            {
                                tx.Rollback();
                                return Json(new { success = false, message = "Error processing stock transfer" });
                            }
                        }

                        // If no valid items were added, rollback and return error
                        if (addedItemCount == 0)
                        {
                            tx.Rollback();
                            return Json(new { success = false, message = "No valid items provided for stock transfer" });
                        }

                        _context.SaveChanges();

                        // Persist stock movements in FW_Stock: reduce from FromGodown and add to ToGodown
                        // Legacy behavior: use same StockId for all FROM entries and a new StockId for all TO entries
                        long nextStockId = 1;
                        var maxId = _context.FwStocks.Select(s => (long?)s.StockId).Max();
                        if (maxId.HasValue)
                        {
                            nextStockId = maxId.Value + 1;
                        }

                        var outStockId = nextStockId;
                        var inStockId = nextStockId + 1;

                        var savedItems = _context.FwStockTransferItems.Where(x => x.TransferId == transfer.TransferId).ToList();
                        // Build TVPs for outgoing (negative quantities) and incoming (positive quantities)
                        var tvpOut = new DataTable();
                        tvpOut.Columns.Add("ItemId", typeof(int));
                        tvpOut.Columns.Add("ItemTypeId", typeof(int));
                        tvpOut.Columns.Add("Quantity", typeof(int));

                        var tvpIn = new DataTable();
                        tvpIn.Columns.Add("ItemId", typeof(int));
                        tvpIn.Columns.Add("ItemTypeId", typeof(int));
                        tvpIn.Columns.Add("Quantity", typeof(int));

                        foreach (var ti in savedItems)
                        {
                            tvpOut.Rows.Add(ti.ItemId, ti.ItemTypeId, -Math.Abs(ti.QtyLowest));
                            tvpIn.Rows.Add(ti.ItemId, ti.ItemTypeId, Math.Abs(ti.QtyLowest));
                        }

                        // Call USP_DML_Stock for outgoing (FromGodown) and incoming (ToGodown)
                        using var conn = _context.Database.GetDbConnection();
                        if (conn.State != ConnectionState.Open) conn.Open();

                        // OUTGOING
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx.GetDbTransaction();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "USP_DML_Stock";

                            var tvpParam = new SqlParameter("@PTVP_Stock", SqlDbType.Structured)
                            {
                                TypeName = "dbo.TVP_Stock",
                                Value = tvpOut
                            };
                            cmd.Parameters.Add(tvpParam);
                            cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = currentUserId });
                            var outResult = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(outResult);
                            cmd.Parameters.Add(new SqlParameter("@StockFrom", SqlDbType.VarChar, 50) { Value = "Stock Transfer" });
                            cmd.Parameters.Add(new SqlParameter("@DMLType", SqlDbType.Char, 1) { Value = isEdit ? "U" : "I" });
                            cmd.Parameters.Add(new SqlParameter("@ReceiptId", SqlDbType.BigInt) { Value = outStockId });
                            cmd.Parameters.Add(new SqlParameter("@InvoiceNo", SqlDbType.VarChar, 50) { Value = "stock transfer (outgoing)" + transfer.TransferId });
                            cmd.Parameters.Add(new SqlParameter("@ArrivalDt", SqlDbType.Date) { Value = DBNull.Value });
                            cmd.Parameters.Add(new SqlParameter("@Comments", SqlDbType.VarChar, 50) { Value = $"Transfer {transfer.TransferId} to Godown {transfer.ToGodown}" });
                            cmd.Parameters.Add(new SqlParameter("@GodownId", SqlDbType.Int) { Value = transfer.FromGodown });

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                                await _errorLogService.LogErrorAsync(
                                    userId,
                                    "BillingController.StockTransfer.USP_DML_Stock_Outgoing",
                                    $"TransferId={transfer.TransferId}, FromGodown={transfer.FromGodown}",
                                    ex);
                                System.Diagnostics.Debug.WriteLine($"USP_DML_Stock (outgoing) threw: {ex.Message}");
                                tx.Rollback();
                                return Json(new { success = false, message = "Error persisting stock (outgoing)", detail = ex.Message });
                            }

                            var r1 = outResult.Value != null ? Convert.ToInt32(outResult.Value) : 0;
                            System.Diagnostics.Debug.WriteLine($"USP_DML_Stock outgoing result: {r1}");
                            // Stored proc semantics: 0 indicates failure (invoice exists for insert), non-zero indicates success
                            if (r1 == 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"USP_DML_Stock outgoing returned zero (failure): {r1}");
                                tx.Rollback();
                                return Json(new { success = false, message = "Error persisting stock (outgoing) - invoice/receipt conflict", result = r1 });
                            }
                        }

                        // INCOMING
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx.GetDbTransaction();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "USP_DML_Stock";

                            var tvpParam2 = new SqlParameter("@PTVP_Stock", SqlDbType.Structured)
                            {
                                TypeName = "dbo.TVP_Stock",
                                Value = tvpIn
                            };
                            cmd.Parameters.Add(tvpParam2);
                            cmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = currentUserId });
                            var outResult2 = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
                            cmd.Parameters.Add(outResult2);
                            cmd.Parameters.Add(new SqlParameter("@StockFrom", SqlDbType.VarChar, 50) { Value = "Stock Transfer" });
                            cmd.Parameters.Add(new SqlParameter("@DMLType", SqlDbType.Char, 1) { Value = isEdit ? "U" : "I" });
                            cmd.Parameters.Add(new SqlParameter("@ReceiptId", SqlDbType.BigInt) { Value = inStockId });
                            cmd.Parameters.Add(new SqlParameter("@InvoiceNo", SqlDbType.VarChar, 50) { Value = "stock transfer (incoming)" + transfer.TransferId });
                            cmd.Parameters.Add(new SqlParameter("@ArrivalDt", SqlDbType.Date) { Value = DBNull.Value });
                            cmd.Parameters.Add(new SqlParameter("@Comments", SqlDbType.VarChar, 50) { Value = $"Transfer {transfer.TransferId} from Godown {transfer.FromGodown}" });
                            cmd.Parameters.Add(new SqlParameter("@GodownId", SqlDbType.Int) { Value = transfer.ToGodown });

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                                await _errorLogService.LogErrorAsync(
                                    userId,
                                    "BillingController.StockTransfer.USP_DML_Stock_Incoming",
                                    $"TransferId={transfer.TransferId}, ToGodown={transfer.ToGodown}",
                                    ex);
                                System.Diagnostics.Debug.WriteLine($"USP_DML_Stock (incoming) threw: {ex.Message}");
                                tx.Rollback();
                                return Json(new { success = false, message = "Error persisting stock (incoming)", detail = ex.Message });
                            }

                            var r2 = outResult2.Value != null ? Convert.ToInt32(outResult2.Value) : 0;
                            System.Diagnostics.Debug.WriteLine($"USP_DML_Stock incoming result: {r2}");
                            // Stored proc semantics: 0 indicates failure (no records for update), non-zero indicates success
                            if (r2 == 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"USP_DML_Stock incoming returned zero (failure): {r2}");
                                tx.Rollback();
                                return Json(new { success = false, message = "Error persisting stock (incoming) - invoice/receipt conflict", result = r2 });
                            }
                        }
                        tx.Commit();

                        return Json(new { success = true, message = "Saved", transferId = transfer.TransferId });
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                        await _errorLogService.LogErrorAsync(
                            userId,
                            "BillingController.StockTransfer.Transaction",
                            $"FromGodown={request.FromGodown}, ToGodown={request.ToGodown}, TransferId={request.TransferId}",
                            ex);
                        System.Diagnostics.Debug.WriteLine($"Error saving stock transfer: {ex.Message}");
                        return Json(new { success = false, message = "Error persisting stock transfer" });
                    }
                }
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.StockTransfer",
                    $"FromGodown={request.FromGodown}, ToGodown={request.ToGodown}, TransferId={request.TransferId}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error saving stock transfer: {ex.Message}");
                return Json(new { success = false, message = "Error saving stock transfer" });
            }
        }

        // GET: /Billing/GetItems?q=search&customerType=1
        [HttpGet]
        public async Task<IActionResult> GetItems(string q, int? godownId, int? customerType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return Json(new { success = true, data = new List<ItemSearchDto>() });

                var sqlParams = new[] {
                    new SqlParameter("@SearchString", System.Data.SqlDbType.VarChar, 100) { Value = (object)q ?? DBNull.Value },
                    new SqlParameter("@CustomerType", System.Data.SqlDbType.Int) { Value = (object)(customerType ?? 1) },
                    new SqlParameter("@Price", System.Data.SqlDbType.Int) {  Value = (object)0 }
                };

                var items = await _context.Set<ItemSearchDto>()
                    .FromSqlRaw("EXEC USP_GET_ItemsBySearch @SearchString, @CustomerType, @Price", sqlParams)
                    .AsNoTracking()
                    .ToListAsync();

                if (godownId.HasValue)
                {
                    // Filter items to those that have any stock rows for the requested godown
                    var gid = godownId.Value;
                    var itemIdsInGodown = await _context.FwStocks
                        .Where(s => s.GodownId == gid && s.Quantity != 0)
                        .Select(s => s.ItemId)
                        .Distinct()
                        .ToListAsync();

                    items = items.Where(i => itemIdsInGodown.Contains(i.ItemId)).ToList();
                }

                return Json(new { success = true, data = items });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "BillingController.GetItems",
                    $"SearchQuery={q}, GodownId={godownId}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error fetching items: {ex.Message}");
                return Json(new { success = false, message = "Error fetching items" });
            }
        }

        // GET: /Billing/GetItemStockDesc?itemId=123&quantity=456
        // NOTE: This method is inherited from BaseController to support stock description lookups
        // across multiple controllers (Billing, Stock, etc.)
        // To use: Call base.GetItemStockDesc(itemId, quantity) or access via AJAX from view

        public IActionResult ViewStockTransfers()
        {
            LoadMenusFromSession();
            // Provide godowns to view for filter selects
            ViewBag.Godowns = _context.FwMasterGodowns.AsNoTracking().ToList();
            return View();
        }
        [HttpGet]
        public IActionResult GetStockTransfers(int page = 1, int size = 10, string sidx = "transferId", string sord = "desc", int? fromGodown = null, int? toGodown = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = (from t in _context.FwStockTransfers
                         join f in _context.FwMasterGodowns on t.FromGodown equals f.Id
                         join g in _context.FwMasterGodowns on t.ToGodown equals g.Id
                         join u in _context.FwUserDetails on t.CreatedBy equals u.UserId
                         select new StockTransferDto
                         {
                             TransferId = t.TransferId,
                             FromGodown = t.FromGodown,
                             ToGodown = t.ToGodown,
                             FromGodownDesc = f.GodownDescription,
                             ToGodownDesc = g.GodownDescription,
                             CreatedBy = u.UserName,
                             CreatedDate = t.CreatedDate
                         });
            // Apply filters if provided
            if (fromGodown.HasValue)
            {
                query = query.Where(x => x.FromGodown == fromGodown.Value);
            }
            if (toGodown.HasValue)
            {
                query = query.Where(x => x.ToGodown == toGodown.Value);
            }
            if (startDate.HasValue)
            {
                var sd = startDate.Value.Date;
                query = query.Where(x => x.CreatedDate >= sd);
            }
            if (endDate.HasValue)
            {
                var ed = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.CreatedDate <= ed);
            }

            // Apply sorting based on jqGrid sidx and sord parameters
            // sidx = column name to sort by
            // sord = sort order (asc or desc)
            query = ApplySorting(query, sidx, sord);

            var total = query.Count();

            var transfers = query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(t => new
                {
                    t.TransferId,
                    t.FromGodown,
                    t.ToGodown,
                    t.FromGodownDesc,
                    t.ToGodownDesc,
                    t.CreatedBy,
                    CreatedDate = t.CreatedDate.ToString("dd-MM-yyyy HH:mm")
                })
                .AsNoTracking()
                .ToList();

            // Support both Tabulator and jqGrid formats
            var lastPage = (int)Math.Ceiling(total / (double)size);
            return Json(new
            {
                data = transfers,
                last_page = lastPage,
                page = page,
                records = total,
                total = lastPage
            });
        }

        /// <summary>
        /// Helper method to apply sorting to stock transfer query
        /// </summary>
        private IQueryable<StockTransferDto> ApplySorting(IQueryable<StockTransferDto> query, string sortColumn, string sortOrder)
        {
            // Sanitize sort order
            var order = sortOrder?.ToLower() == "asc" ? "asc" : "desc";

            // Apply sorting based on column name
            return sortColumn?.ToLower() switch
            {
                "transferid" => order == "asc" ? query.OrderBy(x => x.TransferId) : query.OrderByDescending(x => x.TransferId),
                "fromgodowndesc" => order == "asc" ? query.OrderBy(x => x.FromGodownDesc) : query.OrderByDescending(x => x.FromGodownDesc),
                "togodowndesc" => order == "asc" ? query.OrderBy(x => x.ToGodownDesc) : query.OrderByDescending(x => x.ToGodownDesc),
                "createdby" => order == "asc" ? query.OrderBy(x => x.CreatedBy) : query.OrderByDescending(x => x.CreatedBy),
                "createddate" => order == "asc" ? query.OrderBy(x => x.CreatedDate) : query.OrderByDescending(x => x.CreatedDate),
                _ => query.OrderByDescending(x => x.TransferId) // Default sorting
            };
        }


    }
}
