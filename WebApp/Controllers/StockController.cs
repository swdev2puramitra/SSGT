using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class StockController : BaseController
    {
        private readonly SsgtContext _context;
        private readonly IErrorLogService _errorLogService;

        public StockController(SsgtContext context, IErrorLogService errorLogService)
        {
            _context = context;
            _errorLogService = errorLogService;
        }

        // GET: /Stock/StockList
        public async Task<IActionResult> StockList()
        {
            try
            {
                LoadMenusFromSession();
                // Provide godowns to view for filter dropdown
                ViewBag.Godowns = _context.FwMasterGodowns
                    .AsNoTracking()
                    .Where(g => g.IsActive == true)
                    .OrderBy(g => g.GodownDescription)
                    .ToList();
                return View();
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "StockController.StockList",
                    null,
                    ex);
                ViewBag.Error = "Error loading stock view";
                ViewBag.Godowns = new List<FwMasterGodown>();
                return View();
            }
        }

        // GET: /Stock/ItemSearch
        public IActionResult ItemSearch()
        {
            LoadMenusFromSession();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetItemStockByGodown(int itemId, int page = 1, int pageSize = 20)
        {
            if (itemId <= 0)
            {
                return Json(new { success = false, data = new List<object>(), total = 0, page, pageSize, totalPages = 0 });
            }

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            try
            {
                var stockByGodown = _context.FwStocks
                    .AsNoTracking()
                    .Where(stock => stock.ItemId == itemId)
                    .GroupBy(stock => stock.GodownId)
                    .Select(group => new
                    {
                        GodownId = group.Key,
                        Quantity = group.Sum(stock => stock.Quantity)
                    })
                    .Where(stock => stock.Quantity > 0);

                var query = from stock in stockByGodown
                            join godown in _context.FwMasterGodowns.AsNoTracking() on stock.GodownId equals godown.Id
                            join item in _context.FwMasterItems.AsNoTracking() on itemId equals item.ItemId
                            join company in _context.FwMasterCompanyNames.AsNoTracking() on item.CompanyId equals company.Id into companyJoin
                            from company in companyJoin.DefaultIfEmpty()
                            join category in _context.FwMasterCategories.AsNoTracking() on item.CategoryId equals category.Id into categoryJoin
                            from category in categoryJoin.DefaultIfEmpty()
                            join subcategory in _context.FwMasterSubCategories.AsNoTracking() on item.SubCategoryId equals subcategory.Id into subcategoryJoin
                            from subcategory in subcategoryJoin.DefaultIfEmpty()
                            select new
                            {
                                item.ItemId,
                                item.ItemCode,
                                item.ItemName,
                                stock.Quantity,
                                GodownName = godown.GodownDescription,
                                CompanyName = company != null ? company.CompanyName : "",
                                CategoryName = category != null ? category.CatDescription : "",
                                SubcategoryName = subcategory != null ? subcategory.SubCatDescription : ""
                            };

                var total = await query.CountAsync();
                var rows = await query
                    .OrderBy(row => row.GodownName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Json(new
                {
                    success = true,
                    data = rows,
                    total,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize)
                });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(userId, "StockController.GetItemStockByGodown", $"ItemId={itemId}", ex);
                return Json(new { success = false, message = "Error fetching item stock", data = new List<object>(), total = 0, page, pageSize, totalPages = 0 });
            }
        }

        // GET: /Stock/GetStockByGodown?godownId=1&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetStockByGodown(
            int godownId,
            int page = 1,
            int pageSize = 10,
            string? search = null,
            int? companyId = null,
            int? categoryId = null,
            int? subcategoryId = null)
        {
            try
            {
                if (godownId <= 0)
                    return Json(new { success = false, message = "Invalid godown ID", data = new List<object>() });

                // Query FwStock with all related master data in efficient joins
                var query = from s in _context.FwStocks
                            join item in _context.FwMasterItems on s.ItemId equals item.ItemId into itemJoin
                            from item in itemJoin.DefaultIfEmpty()
                            join cat in _context.FwMasterCategories on item.CategoryId equals cat.Id into catJoin
                            from cat in catJoin.DefaultIfEmpty()
                            join subcat in _context.FwMasterSubCategories on item.SubCategoryId equals subcat.Id into subcatJoin
                            from subcat in subcatJoin.DefaultIfEmpty()
                            where s.GodownId == godownId
                            select new
                            {
                                s.StockId,
                                s.ItemId,
                                ItemName = item != null ? item.ItemName : "N/A",
                                ItemCode = item != null ? item.ItemCode : "",
                                CompanyId = item != null ? item.CompanyId : 0,
                                CategoryId = item != null ? item.CategoryId : 0,
                                SubcategoryId = item != null ? item.SubCategoryId : 0,
                                CompanyName = item != null ? item.Company.CompanyName : "",
                                s.Quantity,
                                s.GodownId,
                                CategoryName = cat != null ? cat.CatDescription : "",
                                SubcategoryName = subcat != null ? subcat.SubCatDescription : ""
                            };

                if (!string.IsNullOrWhiteSpace(search) && search.Trim().Length >= 2)
                {
                    var searchTerm = search.Trim();
                    query = query.Where(x => x.ItemName.Contains(searchTerm) || x.ItemCode.Contains(searchTerm));
                }

                if (companyId is > 0)
                    query = query.Where(x => x.CompanyId == companyId.Value);

                if (categoryId is > 0)
                    query = query.Where(x => x.CategoryId == categoryId.Value);

                if (subcategoryId is > 0)
                    query = query.Where(x => x.SubcategoryId == subcategoryId.Value);

                // Count total records before pagination
                var total = query.AsNoTracking().Count();

                // Apply pagination and ordering
                var stocks = query
                    .AsNoTracking()
                    .OrderBy(x => x.ItemName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = stocks,
                    total = total,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)total / pageSize)
                });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                await _errorLogService.LogErrorAsync(
                    userId,
                    "StockController.GetStockByGodown",
                    $"GodownId={godownId}, Page={page}",
                    ex);
                System.Diagnostics.Debug.WriteLine($"Error fetching stock by godown: {ex.Message}");
                return Json(new { success = false, message = "Error fetching stock", data = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStockItemSuggestions(int godownId, string? term)
        {
            if (godownId <= 0 || string.IsNullOrWhiteSpace(term) || term.Trim().Length < 2)
                return Json(new { data = new List<object>() });

            var searchTerm = term.Trim();
            var suggestions = await (from stock in _context.FwStocks.AsNoTracking()
                                     join item in _context.FwMasterItems.AsNoTracking() on stock.ItemId equals item.ItemId
                                     where stock.GodownId == godownId &&
                                           (item.ItemName.Contains(searchTerm) || item.ItemCode.Contains(searchTerm))
                                     orderby item.ItemName
                                     select new
                                     {
                                         item.ItemId,
                                         item.ItemName,
                                         item.ItemCode
                                     })
                .Distinct()
                .Take(10)
                .ToListAsync();

            return Json(new { data = suggestions });
        }
    }
}
