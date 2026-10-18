using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class ReportsController : BaseController
    {
        private readonly SsgtContext _context;
        private readonly IErrorLogService _errorLogService;

        public ReportsController(SsgtContext context, IMenuService menuService, IErrorLogService errorLogService)
        {
            _context = context;
            _errorLogService = errorLogService;
        }

        [HttpPost]
        public async Task<IActionResult> SetDispatch([FromBody] WebApp.Models.SetDispatchRequest request)
        {
            try
            {
                if (request == null || request.BillId <= 0)
                    return Json(new { success = false, message = "Invalid request" });

                var bill = await _context.FwBills.FindAsync(request.BillId);
                if (bill == null)
                    return Json(new { success = false, message = "Bill not found" });

                bill.DispatchStatus = request.Status ?? "D";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Dispatch status set.", dispatchStatus = bill.DispatchStatus });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "ReportsController.SetDispatch",
                    $"BillId={request?.BillId}",
                    ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> PackingDashboard(long billId)
        {
            LoadMenusFromSession();

            // Get user role from claims
            var roleClaimValue = User.FindFirst(ClaimTypes.Role)?.Value;
            int.TryParse(roleClaimValue, out var userRole);
            // Get packing details for the bill grouped by godown
            var packingData = await _context.Set<PackingDto>()
                .FromSqlRaw("EXEC USP_GetBillGodownWiseAllocation @BillId = {0}", billId)
                .ToListAsync();

            // The allocation procedure can return one row per dispatch/history join.
            // The dashboard must show one row per bill item and godown; dispatch history
            // is aggregated separately below.
            packingData = packingData
                .GroupBy(x => new { x.BillId, x.ItemId, x.GodownId })
                .Select(group =>
                {
                    var row = group.First();
                    row.AllocatedQty = group.Max(x => x.AllocatedQty);
                    row.GodownStock = group.Max(x => x.GodownStock);
                    return row;
                })
                .ToList();

            var stockDescriptions = new Dictionary<(int ItemId, int Quantity), string>();
            foreach (var packingItem in packingData)
            {
                var key = (packingItem.ItemId, packingItem.AllocatedQty);
                if (!stockDescriptions.TryGetValue(key, out var stockDescription))
                {
                    stockDescription = GetItemStockDescriptionRequest(_context, key.ItemId, key.AllocatedQty);
                    stockDescriptions[key] = stockDescription;
                }

                packingItem.StockDescription = stockDescription;
            }

            if (!packingData.Any())
            {
                ViewBag.Error = "No packing data found for this bill.";
                return View(new List<PackingDto>());
            }

            // Load cumulative packing history for each bill/item/godown.
            try
            {
                var packingHistories = await _context.FwPackingHistories
                    .Where(ph => ph.BillId == billId)
                    .Select(ph => new
                    {
                        ph.BillId,
                        ph.ItemId,
                        ph.GodownId,
                        ph.PackedDateTime,
                        ph.QuantityPacked,
                        PackedByUserName = ph.PackedByUser != null && ph.PackedByUser.UserName != null
                            ? ph.PackedByUser.UserName
                            : "Unknown"
                    })
                    .ToListAsync();

                foreach (var packingItem in packingData)
                {
                    var itemHistory = packingHistories
                        .Where(ph => ph.BillId == packingItem.BillId
                            && ph.ItemId == packingItem.ItemId
                            && ph.GodownId == packingItem.GodownId)
                        .ToList();
                    packingItem.TotalDispatchedQty = itemHistory.Sum(ph => ph.QuantityPacked);
                    packingItem.QuantityPacked = packingItem.TotalDispatchedQty;
                    packingItem.RemainingQty = Math.Max(0, packingItem.AllocatedQty - packingItem.TotalDispatchedQty);

                    var latestHistory = itemHistory.OrderByDescending(ph => ph.PackedDateTime).FirstOrDefault();
                    if (latestHistory != null)
                    {
                        packingItem.PackedByUserName = latestHistory.PackedByUserName;
                        packingItem.PackedDateTime = latestHistory.PackedDateTime;
                    }
                }
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "ReportsController.PackingDashboard",
                    $"BillId={billId}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error loading packing history: {ex.Message}");
                // Continue without packing history if table doesn't exist yet
            }

            // Dispatch details describe the quantity being entered in the current
            // dispatch operation. Do not derive them from the allocation procedure's
            // existing dispatch value; the browser fetches them after a positive
            // dispatch quantity is entered.
            foreach (var packingItem in packingData)
                packingItem.DispatchStockDescription = string.Empty;

            // Get bill header info
            var billNo = packingData.FirstOrDefault()?.BillNo;
            var billInfo = await _context.FwBills
                .Join(_context.FwMasterCustomers,
                    b => b.CustomerId,
                    c => c.CustomerId,
                    (b, c) => new { b.BillId, b.BillNo, CustomerName = c.Name })
                .Where(x => x.BillId == billId)
                .FirstOrDefaultAsync();

            ViewBag.BillId = billId;
            ViewBag.BillNo = billInfo?.BillNo ?? billNo;
            ViewBag.CustomerName = billInfo?.CustomerName ?? "N/A";
            // Expose current user role to the view (used by client-side scripts)
            ViewBag.UserRole = userRole;

            return View(packingData);
        }

        [HttpGet]
        public async Task<IActionResult> ItemStockOrderReport()
        {
            LoadMenusFromSession();
            try
            {
                var stockByItem = await _context.FwStocks.AsNoTracking()
                    .GroupBy(x => x.ItemId)
                    .Select(g => new { ItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                    .ToDictionaryAsync(x => x.ItemId, x => x.Quantity);

                // Ordered quantities for this report are limited to bill items that
                // have been packed. Invoice rows are intentionally excluded.
                var estimateOrders = await _context.FwBillsItems.AsNoTracking()
                    .Where(x => x.IsPacked == true)
                    .GroupBy(x => x.ItemId)
                    .Select(g => new { ItemId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                    .ToListAsync();

                var orderedByItem = estimateOrders
                    .GroupBy(x => x.ItemId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));
                var itemIds = stockByItem.Keys.Concat(orderedByItem.Keys).Distinct().ToList();
                var itemInfo = await _context.FwMasterItems.AsNoTracking()
                    .Where(x => itemIds.Contains(x.ItemId))
                    .Select(x => new { x.ItemId, x.ItemCode, x.ItemName })
                    .ToDictionaryAsync(x => x.ItemId);

                var rows = new List<ItemStockOrderReportRow>();
                foreach (var itemId in itemIds)
                {
                    var available = stockByItem.TryGetValue(itemId, out var stock) ? stock : 0;
                    var ordered = orderedByItem.TryGetValue(itemId, out var order) ? order : 0;
                    var required = Math.Max(ordered - available, 0);
                    itemInfo.TryGetValue(itemId, out var info);
                    rows.Add(new ItemStockOrderReportRow
                    {
                        ItemId = itemId,
                        ItemCode = info?.ItemCode ?? string.Empty,
                        ItemName = info?.ItemName ?? $"Item {itemId}",
                        AvailableQuantity = available,
                        AvailableDetails = GetItemStockDescriptionRequest(_context, itemId, Math.Max(available, 0)),
                        OrderedQuantity = ordered,
                        OrderedDetails = GetItemStockDescriptionRequest(_context, itemId, ordered),
                        RequiredQuantity = required,
                        RequiredDetails = GetItemStockDescriptionRequest(_context, itemId, required)
                    });
                }

                var orderedRows = rows.OrderBy(x => x.ItemName).ToList();
                for (var index = 0; index < orderedRows.Count; index++)
                    orderedRows[index].SlNo = index + 1;
                ViewBag.ReportRows = orderedRows;
                return View();
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(User?.FindFirst("UserId")?.Value ?? "Unknown", "ReportsController.ItemStockOrderReport", null, ex);
                ViewBag.ReportRows = new List<ItemStockOrderReportRow>();
                ViewBag.Error = "Unable to load the stock and order report.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDispatchStatus([FromBody] UpdateDispatchRequest request)
        {
            try
            {
                if (request == null || request.BillId <= 0)
                    return Json(new { success = false, message = "Invalid request" });

                var bill = await _context.FwBills.FindAsync(request.BillId);
                if (bill == null)
                    return Json(new { success = false, message = "Bill not found" });

                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out var currentUserId))
                    return Json(new { success = false, message = "Invalid user context" });

                var allocationData = await _context.Set<PackingDto>()
                    .FromSqlRaw("EXEC USP_GetBillGodownWiseAllocation @BillId = {0}", request.BillId)
                    .AsNoTracking()
                    .ToListAsync();
                var billItems = await _context.FwBillsItems
                    .Where(x => x.BillId == request.BillId)
                    .ToDictionaryAsync(x => x.ItemId, x => x.Quantity);
                var existingHistory = await _context.FwPackingHistories
                    .Where(x => x.BillId == request.BillId)
                    .ToListAsync();

                var requestItems = request.PackedItems ?? new List<PackedItem>();
                var dispatchedRows = requestItems
                    .Select(item => new
                    {
                        Item = item,
                        Quantity = item.DispatchQty > 0 ? item.DispatchQty : item.Qty
                    })
                    .Where(x => x.Quantity > 0)
                    .ToList();
                if (dispatchedRows.Count == 0)
                    return Json(new { success = false, message = "Enter at least one positive dispatch quantity." });

                await using var transaction = await _context.Database.BeginTransactionAsync();
                var dispatchedByGodown = existingHistory
                    .GroupBy(x => (x.ItemId, x.GodownId))
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityPacked));
                var dispatchedByItem = existingHistory
                    .GroupBy(x => x.ItemId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityPacked));

                foreach (var dispatchedRow in dispatchedRows)
                {
                    var item = dispatchedRow.Item;
                    var dispatchQty = dispatchedRow.Quantity;
                    var allocation = allocationData.FirstOrDefault(x => x.ItemId == item.ItemId && x.GodownId == item.GodownId);
                    if (allocation == null)
                        return Json(new { success = false, message = $"No allocation found for item {item.ItemId} in godown {item.GodownId}." });

                    var itemKey = (item.ItemId, item.GodownId);
                    var priorGodownQty = dispatchedByGodown.TryGetValue(itemKey, out var godownQty) ? godownQty : 0;
                    if (priorGodownQty + dispatchQty > allocation.AllocatedQty)
                        return Json(new { success = false, message = $"Dispatch quantity exceeds the allocation for item {item.ItemId} in godown {item.GodownId}." });

                    if (!billItems.TryGetValue(item.ItemId, out var billQty))
                        return Json(new { success = false, message = $"Bill item {item.ItemId} was not found." });

                    var priorBillQty = dispatchedByItem.TryGetValue(item.ItemId, out var totalQty) ? totalQty : 0;
                    if (priorBillQty + dispatchQty > billQty)
                        return Json(new { success = false, message = $"Dispatch quantity exceeds the bill quantity for item {item.ItemId}." });

                    var cumulativeGodownQty = priorGodownQty + dispatchQty;
                    var packingHistory = new FwPackingHistory
                    {
                        BillId = request.BillId,
                        ItemId = item.ItemId,
                        GodownId = item.GodownId,
                        AllocatedQty = allocation.AllocatedQty,
                        QuantityPacked = dispatchQty,
                        DispatchType = cumulativeGodownQty == allocation.AllocatedQty ? "Full" : "Partial",
                        PackedByUserId = currentUserId,
                        PackedDateTime = DateTime.Now,
                        PackingStatus = cumulativeGodownQty == allocation.AllocatedQty ? "C" : "P",
                        CreatedDateTime = DateTime.Now
                    };
                    _context.FwPackingHistories.Add(packingHistory);
                    dispatchedByGodown[itemKey] = cumulativeGodownQty;
                    dispatchedByItem[item.ItemId] = priorBillQty + dispatchQty;
                }

                await _context.SaveChangesAsync();

                var finalHistory = await _context.FwPackingHistories
                    .Where(x => x.BillId == request.BillId)
                    .GroupBy(x => x.ItemId)
                    .Select(g => new { ItemId = g.Key, Quantity = g.Sum(x => x.QuantityPacked) })
                    .ToDictionaryAsync(x => x.ItemId, x => x.Quantity);
                foreach (var billItem in await _context.FwBillsItems.Where(x => x.BillId == request.BillId).ToListAsync())
                    billItem.IsPacked = finalHistory.TryGetValue(billItem.ItemId, out var quantity) && quantity >= billItem.Quantity;

                bill.DispatchStatus = billItems.All(x => finalHistory.TryGetValue(x.Key, out var quantity) && quantity >= x.Value) ? "C" : "P";
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, message = "Dispatch recorded successfully", dispatchStatus = bill.DispatchStatus });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "ReportsController.UpdatePackingStatus",
                    $"BillId={request?.BillId}",
                    ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
