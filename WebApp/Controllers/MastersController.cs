using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using WebApp.Helpers;

namespace WebApp.Controllers;

[Authorize]
public sealed class MastersController : BaseController
{
    private const int InventoryPageSize = 20;
    private const int MaximumPageSize = 100;
    private const decimal MaximumPrice = 99_999_999.99m;
    private readonly SsgtContext _context;

    public MastersController(SsgtContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> AddStock(CancellationToken cancellationToken = default)
    {
        LoadMenusFromSession();
        ViewBag.Godowns = await _context.FwMasterGodowns
            .AsNoTracking()
            .Where(g => g.IsActive == true)
            .OrderBy(g => g.GodownDescription)
            .ToListAsync(cancellationToken);
        ViewBag.Companies = await _context.FwMasterCompanyNames
            .AsNoTracking()
            .Where(c => c.IsActive == true)
            .OrderBy(c => c.CompanyName)
            .ToListAsync(cancellationToken);
        return View();
    }

    [HttpGet]
    public IActionResult ViewStockList()
    {
        LoadMenusFromSession();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetStockItemTypes(int itemId, CancellationToken cancellationToken = default)
    {
        if (itemId <= 0)
        {
            return Json(new { success = false, data = Array.Empty<object>() });
        }

        var types = await (from conversion in _context.FwMasterItemsConversions.AsNoTracking()
                           join type in _context.FwMasterItemTypes.AsNoTracking()
                               on conversion.ItemTypeId equals type.Id
                           where conversion.ItemId == itemId && conversion.IsActive == true
                           orderby type.TypeName
                           select new
                           {
                               itemTypeId = conversion.ItemTypeId,
                               typeName = type.TypeName,
                               qtyPerType = conversion.QtyPerType
                           })
            .ToListAsync(cancellationToken);

        return Json(new { success = true, data = types });
    }

    [HttpGet]
    public async Task<IActionResult> GetStockList(
        int page = 1,
        int pageSize = 15,
        string? stockFrom = null,
        string? invoiceNo = null,
        int? itemId = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = from stock in _context.FwStocks.AsNoTracking()
                    select new StockListRowDto
                    {
                        StockId = stock.StockId,
                        StockFrom = stock.StockFrom,
                        Godown = stock.Godown.GodownDescription,
                        ReceivedDate = stock.CreatedDate,
                        GodownId = stock.GodownId,
                        InvoiceNo = stock.InvoiceNo,
                        ArrivalDate = stock.ArrivalDt,
                        Comments = stock.Comments
                    };

        query = query.Distinct();

        if (!string.IsNullOrWhiteSpace(stockFrom))
        {
            var value = stockFrom.Trim();
            query = query.Where(row => row.StockFrom != null && row.StockFrom.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(invoiceNo))
        {
            var value = invoiceNo.Trim();
            query = query.Where(row => row.InvoiceNo != null && row.InvoiceNo.Contains(value));
        }

        if (itemId is > 0)
        {
            query = query.Where(row => _context.FwStocks.Any(stock =>
                stock.StockId == row.StockId && stock.ItemId == itemId.Value));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(row => row.StockId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Json(new StockListResponse
        {
            Data = rows,
            Page = page,
            Total = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetStockReceipt(long stockId, CancellationToken cancellationToken = default)
    {
        var stocks = await (from row in _context.FwStocks.AsNoTracking()
                            join item in _context.FwMasterItems.AsNoTracking() on row.ItemId equals item.ItemId
                            where row.StockId == stockId
                            select new
                            {
                                stockId = row.StockId,
                                godownId = row.GodownId,
                                itemId = row.ItemId,
                                itemCode = item.ItemCode,
                                itemName = item.ItemName,
                                quantity = row.Quantity,
                                itemTypeId = _context.FwMasterItemsConversions
                                    .Where(conversion => conversion.ItemId == row.ItemId && conversion.IsActive == true)
                                    .Select(conversion => conversion.ItemTypeId)
                                    .FirstOrDefault(),
                                itemTypeName = (from conversion in _context.FwMasterItemsConversions
                                                join type in _context.FwMasterItemTypes on conversion.ItemTypeId equals type.Id
                                                where conversion.ItemId == row.ItemId && conversion.IsActive == true
                                                select type.TypeName).FirstOrDefault(),
                                stockFrom = row.StockFrom,
                                invoiceNo = row.InvoiceNo,
                                arrivalDate = row.ArrivalDt,
                                comments = row.Comments
                            })
            .ToListAsync(cancellationToken);

        return stocks.Count == 0
            ? NotFound(new { message = "Stock entry not found." })
            : Json(new
            {
                stockId,
                godownId = stocks[0].godownId,
                stockFrom = stocks[0].stockFrom,
                invoiceNo = stocks[0].invoiceNo,
                arrivalDate = stocks[0].arrivalDate,
                comments = stocks[0].comments,
                items = stocks.Select(row => new
                {
                    row.itemId,
                    row.itemCode,
                    row.itemName,
                    row.quantity,
                    row.itemTypeId,
                    row.itemTypeName
                })
            });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveStock(
        [FromBody] StockReceiptRequest? request,
        CancellationToken cancellationToken = default)
    {
        if (request == null || request.GodownId <= 0 || request.Items.Count == 0)
        {
            return BadRequest(new { message = "Godown and at least one valid stock item are required." });
        }

        if (string.IsNullOrWhiteSpace(request.StockFrom) || string.IsNullOrWhiteSpace(request.InvoiceNo))
        {
            return BadRequest(new { message = "Stock From and Invoice No are required." });
        }

        //if (request.StockId is > 0 && request.Items.Count != 1)
        //{
        //    return BadRequest(new { message = "An existing stock entry can contain only one item." });
        //}

        if (request.Items.GroupBy(item => item.ItemId).Any(group => group.Count() > 1))
        {
            return BadRequest(new { message = "The same item cannot be added more than once in one stock entry." });
        }

        var godownExists = await _context.FwMasterGodowns.AnyAsync(g => g.Id == request.GodownId && g.IsActive == true, cancellationToken);
        var itemIds = request.Items.Select(item => item.ItemId).Distinct().ToArray();
        var itemCount = await _context.FwMasterItems
            .CountAsync(item => itemIds.Contains(item.ItemId) && item.IsActive == true, cancellationToken);
        var conversions = await _context.FwMasterItemsConversions
            .AsNoTracking()
            .Where(row => itemIds.Contains(row.ItemId) && row.IsActive == true)
            .Select(row => new { row.ItemId, row.ItemTypeId, row.QtyPerType })
            .ToListAsync(cancellationToken);

        if (!godownExists || itemCount != itemIds.Length || request.Items.Any(item => item.ItemId <= 0 || item.ItemTypeId <= 0 || item.Quantity <= 0))
        {
            return BadRequest(new { message = "The selected godown or one of the stock items is invalid." });
        }

        var conversionMap = conversions.ToDictionary(row => (row.ItemId, row.ItemTypeId), row => row.QtyPerType.GetValueOrDefault(1));
        if (request.Items.Any(item => !conversionMap.ContainsKey((item.ItemId, item.ItemTypeId)) ||
                                      conversionMap[(item.ItemId, item.ItemTypeId)] <= 0 ||
                                      (!request.IsBaseQuantity || request.StockId is not > 0) &&
                                      item.Quantity > int.MaxValue / conversionMap[(item.ItemId, item.ItemTypeId)]))
        {
            return BadRequest(new { message = "One or more item types are invalid or have a quantity that is too large." });
        }

        var userId = int.TryParse(User.FindFirstValue("UserId"), out var parsedUserId) ? parsedUserId : 0;
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        try
        {
            var nextStockId = (await _context.FwStocks.Select(row => (long?)row.StockId).MaxAsync(cancellationToken) ?? 0) + 1;
            var entryStockId = request.StockId ?? nextStockId;
            var existingStocks = request.StockId is > 0
                ? await _context.FwStocks.Where(row => row.StockId == request.StockId.Value).ToListAsync(cancellationToken)
                : new List<FwStock>();

            if (request.StockId is > 0 && existingStocks.Count == 0)
            {
                throw new InvalidOperationException("Stock entry not found.");
            }

            if (existingStocks.Count > 0 && existingStocks.Any(stock => stock.GodownId != request.GodownId))
            {
                return BadRequest(new { message = "The godown cannot be changed when editing stock." });
            }

            foreach (var item in request.Items)
            {
                var stock = existingStocks.FirstOrDefault(row => row.ItemId == item.ItemId)
                    ?? new FwStock
                    {
                        StockId = entryStockId,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    };
                var qtyPerType = conversionMap[(item.ItemId, item.ItemTypeId)];

                stock.StockId = entryStockId;
                stock.GodownId = request.GodownId;
                stock.ItemId = item.ItemId;
                stock.Quantity = request.IsBaseQuantity
                    ? item.Quantity
                    : checked(item.Quantity * qtyPerType);
                stock.StockFrom = request.StockFrom.Trim();
                stock.InvoiceNo = request.InvoiceNo.Trim();
                stock.ArrivalDt = request.ArrivalDate;
                stock.Comments = string.IsNullOrWhiteSpace(request.Comments) ? null : request.Comments.Trim();

                if (!existingStocks.Contains(stock))
                {
                    _context.FwStocks.Add(stock);
                }
            }

            var requestedItemIds = request.Items.Select(item => item.ItemId).ToHashSet();
            var deletedStocks = existingStocks
                .Where(stock => !requestedItemIds.Contains(stock.ItemId))
                .ToList();
            if (deletedStocks.Count > 0)
            {
                _context.FwStocks.RemoveRange(deletedStocks);
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Json(new { success = true, stockId = entryStockId });
        }
        catch (InvalidOperationException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return NotFound(new { message = ex.Message });
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            return StatusCode(500, new { message = "Unable to save stock entry." });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStock(long stockId, CancellationToken cancellationToken = default)
    {
        var stocks = await _context.FwStocks.Where(row => row.StockId == stockId).ToListAsync(cancellationToken);
        if (stocks.Count == 0)
        {
            return NotFound(new { message = "Stock entry not found." });
        }

        _context.FwStocks.RemoveRange(stocks);
        await _context.SaveChangesAsync(cancellationToken);
        return Json(new { success = true });
    }

    // InventoryCompare and direct stored-proc diagnostics removed — EF-only implementation retained

    [HttpGet]
    public async Task<IActionResult> GetCompanyMargins(int companyId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (companyId <= 0) return Json(new { success = false, message = "Invalid companyId" });

            var comp = await _context.FwMasterCompanyNames
                .AsNoTracking()
                .Where(c => c.Id == companyId && c.IsActive == true)
                .Select(c => new { semi = c.SemiWholeSale ?? 0m, retail = c.Retail ?? 0m })
                .FirstOrDefaultAsync(cancellationToken);

            if (comp == null) return Json(new { success = false, message = "Company not found" });

            return Json(new { success = true, data = new { semiWholeSale = comp.semi, retail = comp.retail } });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching company margins: {ex.Message}");
            return Json(new { success = false, message = "Error fetching company margins" });
        }
    }


    public IActionResult Inventory()
    {
        LoadMenusFromSession();
        ViewBag.CanUpdatePrices = User.FindFirstValue(ClaimTypes.Role) == "1";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SuggestItems(string? term, CancellationToken cancellationToken)
    {
        var searchTerm = term?.Trim() ?? string.Empty;
        if (searchTerm.Length < 2)
        {
            return Json(Array.Empty<InventoryAutocompleteOptionDto>());
        }

        var options = await _context.FwMasterItems
            .AsNoTracking()
            .Where(item => item.IsActive == true &&
                (EF.Functions.Like(item.ItemName, $"%{searchTerm}%") ||
                 EF.Functions.Like(item.ItemCode, $"%{searchTerm}%")))
            .OrderBy(item => item.ItemName)
            .Take(15)
            .Select(item => new InventoryAutocompleteOptionDto
            {
                Id = item.ItemId,
                Label = item.ItemName + " (" + item.ItemCode + ")",
                Value = item.ItemName
            })
            .ToListAsync(cancellationToken);

        return Json(options);
    }

    [HttpGet]
    public async Task<IActionResult> SuggestCompanies(string? term, CancellationToken cancellationToken)
    {
        var searchTerm = term?.Trim() ?? string.Empty;
        var options = await _context.FwMasterCompanyNames
            .AsNoTracking()
            .Where(company => company.IsActive == true &&
                (searchTerm.Length == 0 || EF.Functions.Like(company.CompanyName, $"%{searchTerm}%")))
            .OrderBy(company => company.CompanyName)
            .Take(15)
            .Select(company => new InventoryAutocompleteOptionDto
            {
                Id = company.Id,
                Label = company.CompanyName,
                Value = company.CompanyName
            })
            .ToListAsync(cancellationToken);

        return Json(options);
    }

    [HttpGet]
    public async Task<IActionResult> SuggestCategories(string? term, CancellationToken cancellationToken)
    {
        var searchTerm = term?.Trim() ?? string.Empty;
        var options = await _context.FwMasterCategories
            .AsNoTracking()
            .Where(category => category.IsActive == true &&
                (searchTerm.Length == 0 || EF.Functions.Like(category.CatDescription, $"%{searchTerm}%")))
            .OrderBy(category => category.CatDescription)
            .Take(15)
            .Select(category => new InventoryAutocompleteOptionDto
            {
                Id = category.Id,
                Label = category.CatDescription,
                Value = category.CatDescription
            })
            .ToListAsync(cancellationToken);

        return Json(options);
    }

    [HttpGet]
    public async Task<IActionResult> SuggestSubcategories(
        string? term,
        int? categoryId,
        CancellationToken cancellationToken)
    {
        var searchTerm = term?.Trim() ?? string.Empty;
        var query = _context.FwMasterSubCategories
            .AsNoTracking()
            .Where(subcategory => subcategory.IsActive == true);

        if (categoryId is > 0)
        {
            query = query.Where(subcategory => subcategory.CategoryId == categoryId);
        }

        if (searchTerm.Length > 0)
        {
            query = query.Where(subcategory => EF.Functions.Like(subcategory.SubCatDescription, $"%{searchTerm}%"));
        }

        var options = await query
            .OrderBy(subcategory => subcategory.SubCatDescription)
            .Take(15)
            .Select(subcategory => new InventoryAutocompleteOptionDto
            {
                Id = subcategory.Id,
                Label = subcategory.SubCatDescription,
                Value = subcategory.SubCatDescription
            })
            .ToListAsync(cancellationToken);

        return Json(options);
    }

    [HttpGet]
    public async Task<IActionResult> InventoryData(
        int page = 1,
        int pageSize = InventoryPageSize,
        string? itemName = null,
        int? itemId = null,
        int? companyId = null,
        int? categoryId = null,
        int? subcategoryId = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaximumPageSize);

        var query = _context.FwMasterItems
            .AsNoTracking()
            .Where(item => item.IsActive == true);

        if (itemId is > 0)
        {
            query = query.Where(item => item.ItemId == itemId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(itemName))
        {
            var searchTerm = itemName.Trim();
            query = query.Where(item => EF.Functions.Like(item.ItemName, $"%{searchTerm}%") ||
                                        EF.Functions.Like(item.ItemCode, $"%{searchTerm}%"));
        }

        if (companyId is > 0)
        {
            query = query.Where(item => item.CompanyId == companyId.Value);
        }

        if (categoryId is > 0)
        {
            query = query.Where(item => item.CategoryId == categoryId.Value);
        }

        if (subcategoryId is > 0)
        {
            query = query.Where(item => item.SubCategoryId == subcategoryId.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        page = Math.Min(page, totalPages);
        var skip = (page - 1) * pageSize;

        var selectedItems = await query
            .OrderBy(item => item.ItemCode)
            .ThenBy(item => item.ItemId)
            .Skip(skip)
            .Take(pageSize)
            .Select(item => new
            {
                item.ItemId,
                item.ItemCode,
                item.ItemName,
                item.WholeSalePrice,
                item.SemiWholeSalePrice,
                item.RetailPrice,
                item.ConversionId,
                item.CompanyId,
                SemiWholeSaleMargin = item.Company.SemiWholeSale ?? 0m,
                RetailMargin = item.Company.Retail ?? 0m,
                CompanyName = item.Company.CompanyName,
                CategoryName = item.Category.CatDescription,
                SubCategoryName = item.SubCategory.SubCatDescription,
                StockQuantity = _context.FwStocks
                    .Where(stock => stock.ItemId == item.ItemId)
                    .Select(stock => (int?)stock.Quantity)
                    .Sum() ?? 0,
                PackedQuantity = _context.FwBillsItems
                    .Where(billItem => billItem.ItemId == item.ItemId && billItem.IsPacked == true)
                    .Select(billItem => (int?)billItem.Quantity)
                    .Sum() ?? 0,
            })
            .ToListAsync(cancellationToken);

        var selectedItemIds = selectedItems.Select(item => item.ItemId).ToArray();
        // The original single-query join could not be translated by EF Core in some schemas.
        // Load the minimal data in separate queries and perform the join in-memory to avoid translation issues
        var conversionRows = new List<InventoryConversionDescriptionRow>();
        // prepare holders for conversion metadata used later
        List<(int ConversionId, int ItemOrder, int ItemTypeId)> conversions = new();
        List<WebApp.Models.FwMasterItemType> itemTypes = new();
        if (selectedItemIds.Length > 0)
        {
            // load item conversions for selected items
            var itemConversions = await _context.FwMasterItemsConversions
                .AsNoTracking()
                .Where(ic => selectedItemIds.Contains(ic.ItemId))
                .ToListAsync(cancellationToken);

            // load the conversion records that might be referenced by these items
            var conversionIdsForItems = selectedItems
                .Where(si => si.ConversionId.HasValue)
                .Select(si => si.ConversionId!.Value)
                .Distinct()
                .ToArray();

            if (conversionIdsForItems.Length == 0)
            {
                conversions = new List<(int ConversionId, int ItemOrder, int ItemTypeId)>();
            }
            else
            {
                conversions = (await _context.FwMasterConversions
                    .AsNoTracking()
                    .Where(c => conversionIdsForItems.Contains(c.ConversionId))
                    .Select(c => new { c.ConversionId, c.ItemOrder, c.ItemTypeId })
                    .ToListAsync(cancellationToken))
                    .Select(x => (x.ConversionId, x.ItemOrder, x.ItemTypeId))
                    .ToList();
            }

            // load item types referenced by the item conversions
            var itemTypeIds = itemConversions.Select(ic => ic.ItemTypeId).Distinct().ToArray();
            itemTypes = itemTypeIds.Length == 0
                ? new List<WebApp.Models.FwMasterItemType>()
                : await _context.FwMasterItemTypes
                    .AsNoTracking()
                    .Where(t => itemTypeIds.Contains(t.Id))
                    .ToListAsync(cancellationToken);

            // perform the join in memory
            foreach (var ic in itemConversions)
            {
                var selItem = selectedItems.FirstOrDefault(si => si.ItemId == ic.ItemId);
                if (selItem == null || selItem.ConversionId == null) continue;

                var conv = conversions.FirstOrDefault(c => c.ConversionId == selItem.ConversionId.Value);
                if (conv == default) continue;

                if (conv.ItemTypeId != ic.ItemTypeId) continue;

                var type = itemTypes.FirstOrDefault(t => t.Id == ic.ItemTypeId);
                var typeName = type?.TypeName ?? string.Empty;

                conversionRows.Add(new InventoryConversionDescriptionRow(
                    ic.ItemId,
                    ic.ItemTypeId,
                    ic.QtyPerType,
                    conv.ItemOrder,
                    typeName));
            }

            // ensure deterministic ordering and deduplication (record equality works)
            conversionRows = conversionRows.Distinct()
                .OrderBy(r => r.ItemId)
                .ThenBy(r => r.ItemOrder)
                .ToList();
        }

        var conversionsByItem = conversionRows
            .GroupBy(row => row.ItemId)
            .ToDictionary(group => group.Key, group => group.ToList());

        // Build lookups for conversions by ConversionId (from FwMasterConversions query) and item types
        var conversionsByConversionId = conversions
            .GroupBy(c => c.ConversionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var itemTypesLookup = itemTypes.ToDictionary(t => t.Id, t => t);

        var conversionIds = selectedItems
            .Where(item => item.ConversionId.HasValue)
            .Select(item => item.ConversionId!.Value)
            .Distinct()
            .ToArray();

        var conversionNames = new Dictionary<int, string>();
        if (conversionIds.Length > 0)
        {
            var conversionRecords = await _context.FwMasterConversions
                .AsNoTracking()
                .Where(conversion => conversionIds.Contains(conversion.ConversionId))
                .Select(conversion => new { conversion.ConversionId, conversion.ConversionName })
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var record in conversionRecords)
            {
                if (!conversionNames.ContainsKey(record.ConversionId))
                {
                    conversionNames[record.ConversionId] = record.ConversionName;
                }
            }
        }

        var rows = selectedItems.Select((item, index) =>
    {
        var netStock = item.StockQuantity - item.PackedQuantity;
        var itemConversions = conversionsByItem.GetValueOrDefault(item.ItemId) ?? new List<InventoryConversionDescriptionRow>();
        // QtyPerType: first conversion row per ItemId ordered by ItemTypeId (ROW_NUMBER partition semantics)
        var firstRow = itemConversions.OrderBy(c => c.ItemTypeId).FirstOrDefault();
        var qtyPerTypeForContains = firstRow?.QtyPerType;

        // Unit label selection: prefer FwMasterConversions row with ItemOrder == 2 for the item's ConversionId.
        // Use the preloaded conversions list (conversionsByConversionId) to find the ItemTypeId, then lookup TypeName.
        string? stockDisplayType = null;
        if (item.ConversionId.HasValue && conversionsByConversionId.TryGetValue(item.ConversionId.Value, out var convList) && convList.Count > 0)
        {
            // try to find ItemOrder == 2 first
            var convOrder2 = convList.FirstOrDefault(c => c.ItemOrder == 2);
            if (!convOrder2.Equals(default))
            {
                itemTypesLookup.TryGetValue(convOrder2.ItemTypeId, out var it);
                stockDisplayType = it?.TypeName;
            }

            // fallback: lowest ItemOrder in convList
            if (stockDisplayType == null)
            {
                var fallback = convList.OrderBy(c => c.ItemOrder).FirstOrDefault();
                if (!fallback.Equals(default))
                {
                    itemTypesLookup.TryGetValue(fallback.ItemTypeId, out var it2);
                    stockDisplayType = it2?.TypeName;
                }
            }
        }

        string contains;
        if (qtyPerTypeForContains.HasValue && !string.IsNullOrWhiteSpace(stockDisplayType))
        {
            contains = $"{qtyPerTypeForContains.Value} {stockDisplayType}'s";
        }
        else if (item.ConversionId.HasValue && conversionNames.TryGetValue(item.ConversionId.Value, out var convName) && !string.IsNullOrWhiteSpace(convName))
        {
            contains = $"1 {convName}";
        }
        else
        {
            contains = string.Empty;
        }

        return new InventoryRowDto
        {
            SlNo = skip + index + 1,
            ItemId = item.ItemId,
            ItemCode = item.ItemCode,
            ItemName = item.ItemName,
            StockPosition = ItemTypeDescriptionHelper.GetItemTypewiseDescription(_context, item.ItemId, netStock),
            WholeSalePrice = item.WholeSalePrice,
            SemiWholeSalePrice = item.SemiWholeSalePrice,
            RetailPrice = item.RetailPrice,
            CompanyId = item.CompanyId,
            SemiWholeSaleMargin = item.SemiWholeSaleMargin,
            RetailMargin = item.RetailMargin,
            CompanyName = item.CompanyName,
            CategoryName = item.CategoryName,
            SubCategoryName = item.SubCategoryName,
            ConversionName = item.ConversionId.HasValue && conversionNames.TryGetValue(item.ConversionId.Value, out var name)
                ? name
                : string.Empty,
            Contains = contains,
            ConversionId = item.ConversionId
        };
    }).ToList();

        return Json(new InventoryGridResponseDto
        {
            Data = rows,
            Page = page,
            TotalPages = totalPages,
            Total = total
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInventoryPrices(
        [FromBody] List<InventoryPriceUpdateDto>? items,
        CancellationToken cancellationToken)
    {
        if (User.FindFirstValue(ClaimTypes.Role) != "1")
        {
            return Forbid();
        }

        if (items is null || items.Count == 0)
        {
            return BadRequest(new { message = "No modified inventory rows were supplied." });
        }

        if (items.Any(item => item.ItemId <= 0 ||
                              item.WholeSalePrice < 0 || item.WholeSalePrice > MaximumPrice ||
                              item.SemiWholeSalePrice < 0 || item.SemiWholeSalePrice > MaximumPrice ||
                              item.RetailPrice < 0 || item.RetailPrice > MaximumPrice) ||
            items.Select(item => item.ItemId).Distinct().Count() != items.Count)
        {
            return BadRequest(new { message = "One or more inventory prices are invalid." });
        }

        var itemIds = items.Select(item => item.ItemId).ToArray();
        var entities = await _context.FwMasterItems
            .Where(item => itemIds.Contains(item.ItemId) && item.IsActive == true)
            .ToDictionaryAsync(item => item.ItemId, cancellationToken);

        if (entities.Count != items.Count)
        {
            return NotFound(new { message = "One or more inventory items were not found." });
        }

        // Prepare a DataTable for TVP parameter
        var table = new DataTable();
        table.Columns.Add(new DataColumn("RowId", typeof(int)));
        table.Columns.Add(new DataColumn("ItemId", typeof(int)));
        table.Columns.Add(new DataColumn("WholeSalePrice", typeof(decimal)));
        table.Columns.Add(new DataColumn("SemiWholeSalePrice", typeof(decimal)));
        table.Columns.Add(new DataColumn("RetailPrice", typeof(decimal)));

        var rowId = 1;
        foreach (var update in items)
        {
            table.Rows.Add(rowId++, update.ItemId, update.WholeSalePrice, update.SemiWholeSalePrice, update.RetailPrice);
        }

        // Execute stored procedure USP_DML_Items_Price with TVP parameter and an output Result
        var conn = _context.Database.GetDbConnection();
        try
        {
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(cancellationToken);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "USP_DML_Items_Price";
            cmd.CommandType = CommandType.StoredProcedure;

            var tvpParam = new SqlParameter("@P_TVP_Items_Update", SqlDbType.Structured)
            {
                TypeName = "dbo.TVP_Items_Update",
                Value = table
            };
            cmd.Parameters.Add(tvpParam);

            var outParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(outParam);
            var dbCmd = (DbCommand)cmd;
            await dbCmd.ExecuteNonQueryAsync(cancellationToken);

            var result = outParam.Value != DBNull.Value ? Convert.ToInt32(outParam.Value) : 0;
            if (result > 0)
            {
                return Ok(new { message = "Inventory prices updated successfully." });
            }
            else
            {
                return StatusCode(500, new { message = "Stored procedure reported failure updating prices." });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error executing price update stored proc: {ex.Message}");
            return StatusCode(500, new { message = "Error updating inventory prices." });
        }
        finally
        {
            if (conn.State == ConnectionState.Open) conn.Close();
        }
    }

    private static string FormatStockPosition(
        int quantity,
        IReadOnlyList<InventoryConversionDescriptionRow> conversions)
    {
        if (quantity <= 0 || conversions.Count == 0)
        {
            return string.Empty;
        }

        var balance = quantity;
        var parts = new List<string>(conversions.Count);
        foreach (var conversion in conversions.OrderBy(row => row.ItemOrder))
        {
            var quantityPerType = conversion.QtyPerType.GetValueOrDefault();
            var divisor = quantityPerType == 0 ? 1 : quantityPerType;
            var convertedQuantity = balance / divisor;
            balance %= divisor;
            // Match legacy formatting (e.g., "Case's", "Box's") including apostrophe
            parts.Add($"{convertedQuantity} {conversion.TypeName}'s");
        }

        return string.Join(" ", parts);
    }

    private sealed record InventoryConversionDescriptionRow(
        int ItemId,
        int ItemTypeId,
        int? QtyPerType,
        int ItemOrder,
        string TypeName);
}
