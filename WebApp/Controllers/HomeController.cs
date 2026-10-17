using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly SsgtContext _context;
        private readonly IErrorLogService _errorLogService;

        public HomeController(IMenuService menuService, SsgtContext context, IErrorLogService errorLogService)
        {
            _menuService = menuService;
            _context = context;
            _errorLogService = errorLogService;
        }

        public IActionResult Index()
        {
            LoadMenusFromSession();
            return View();
        }

        public IActionResult Privacy()
        {
            LoadMenusFromSession();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            LoadMenusFromSession();
            return View();
        }

        /// <summary>
        /// Under Construction page for menu items without URL/FormName
        /// </summary>
        [AllowAnonymous]
        public IActionResult UnderConstruction(string? menuName = null)
        {
            LoadMenusFromSession();
            ViewBag.MenuName = menuName ?? "This Feature";
            return View();
        }

        /// <summary>
        /// Packing Dashboard - displays bills for packing operations
        /// Uses USP_GET_BILLS stored procedure with optional filters
        /// Default filter: shows today's bills if no date is provided
        /// </summary>
        public async System.Threading.Tasks.Task<IActionResult> ViewBills(
            DateTime? createdDate = null,
            string? name = null,
            string? billNo = null,
            long? mobileNo = null,
            int? customerType = null,
            char? isEstimate = null)
        {
            LoadMenusFromSession();

            try
            {
                // Get user role from claims
                var roleClaimValue = User.FindFirst(ClaimTypes.Role)?.Value;
                int.TryParse(roleClaimValue, out var userRole);

                // Default createdDate to today when not specified so initial load is filtered server-side
                createdDate ??= DateTime.Today;

                // Load first page of bills on server so the initial page is populated without an extra client AJAX call
                const int initialPage = 1;
                const int initialPageSize = 10; // match client default pageSize

                var billsQuery = BuildBillsQuery(createdDate, name, billNo, mobileNo, customerType, isEstimate, userRole);
                var total = await billsQuery.CountAsync();
                var pageRows = await billsQuery.OrderByDescending(b => b.BillDate)
                                               .Skip((initialPage - 1) * initialPageSize)
                                               .Take(initialPageSize)
                                               .ToListAsync();

                var items = pageRows.Select((b, idx) => new BillsListDto
                {
                    SlNo = ((initialPage - 1) * initialPageSize) + idx + 1,
                    BillId = b.BillId,
                    BillNo = b.BillNo,
                    CustomerName = b.CustomerName,
                    CustAddress = b.CustAddress,
                    BillAmount = b.TotalBillAmount ?? 0m,
                    BillDate = b.BillDate,
                    PhoneNo = b.PhoneNo,
                    Estimate = b.IsEstimateInvoice == "E" ? "Yes" : "No",
                    CustomerType = b.CustomerTypeText,
                    InvoiceNo = b.InvoiceNo,
                    IsInState = b.IsInState ?? false,
                    WayBill = b.WayBill,
                    InvoiceStatus = b.InvoiceStatus,
                    DispatchStatus = b.DispatchStatus,
                    OrderConfirmed = b.OrderConfirmed,
                    TotalCount = total
                }).ToList();

                ViewBag.Bills = items;
                ViewBag.TotalBillsCount = total;
                ViewBag.SelectedCustomerType = customerType;
                ViewBag.SelectedIsEstimate = isEstimate;
                ViewBag.SelectedCreatedDate = createdDate;
                ViewBag.SelectedName = name;
                ViewBag.SelectedBillNo = billNo;
                ViewBag.SelectedMobileNo = mobileNo;
                // Expose current user role to the view (used by client-side scripts)
                ViewBag.UserRole = userRole;

                // Total already set above when loading initial page

                return View();
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                _errorLogService.LogErrorAsync(
                    userId,
                    "HomeController.ViewBills",
                    null,
                    ex).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine($"Error loading packing dashboard: {ex.Message}");
                ViewBag.Bills = new List<BillsListDto>();
                ViewBag.Error = "Error loading bills. Please try again.";
                return View();
            }
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<IActionResult> GetBillsPage(int page = 1, int pageSize = 15, DateTime? createdDate = null,
            string? name = null, string? billNo = null, long? mobileNo = null, int? customerType = null, char? isEstimate = null)
        {
            try
            {
                var roleClaimValue = User.FindFirst(ClaimTypes.Role)?.Value;
                int.TryParse(roleClaimValue, out var userRole);

                // Build query via EF and return paged result
                var query = BuildBillsQuery(createdDate, name, billNo, mobileNo, customerType, isEstimate, userRole)
                    .Select(b => new BillsQueryItem
                    {
                        BillId = b.BillId,
                        BillNo = b.BillNo,
                        DispatchStatus = b.DispatchStatus,
                         TotalBillAmount = b.TotalBillAmount,
                        BillDate = b.BillDate,
                        IsEstimateInvoice = b.IsEstimateInvoice,
                        CustomerType = b.CustomerType,
                        CustomerId = b.CustomerId,
                        WayBill = b.WayBill,
                        IsInState = b.IsInState,
                        InvoiceNo = b.InvoiceNo,
                        InvoiceStatus = b.InvoiceStatus,
                        PhoneNo = b.PhoneNo,
                        CustomerName = b.CustomerName,
                        CustAddress = b.CustAddress,
                        CustomerTypeText = b.CustomerTypeText
                    });

                var total = await query.CountAsync();
                var pageRows = await query.OrderByDescending(b => b.BillDate)
                                           .Skip((page - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();

                var items = pageRows.Select((b, idx) => new BillsListDto
                {
                    SlNo = ((page - 1) * pageSize) + idx + 1,
                    BillId = b.BillId,
                    BillNo = b.BillNo,
                    CustomerName = b.CustomerName,
                    CustAddress = b.CustAddress,
                    BillAmount = b.TotalBillAmount ?? 0m,
                    BillDate = b.BillDate,
                    PhoneNo = b.PhoneNo,
                    Estimate = b.IsEstimateInvoice == "E" ? "Yes" : "No",
                    CustomerType = b.CustomerTypeText,
                    InvoiceNo = b.InvoiceNo,
                    IsInState = b.IsInState ?? false,
                    WayBill = b.WayBill,
                    InvoiceStatus = b.InvoiceStatus,
                    TotalCount = total,
                    DispatchStatus = b.DispatchStatus,
                    OrderConfirmed = b.OrderConfirmed
                }).ToList();

                return Json(new { success = true, data = items, total = total, page = page, pageSize = pageSize, totalPages = (int)Math.Ceiling((double)total / pageSize) });
            }
            catch (Exception ex)
            {
                var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
                _errorLogService.LogErrorAsync(
                    userId,
                    "HomeController.BillsData",
                    $"Page={page}, PageSize={pageSize}",
                    ex).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine($"Error fetching bills page: {ex.Message}");
                return Json(new { success = false, message = "Error fetching bills" });
            }
        }

        /// <summary>
        /// Build an EF query that returns bill/invoice rows matching filters.
        /// </summary>
        private IQueryable<BillsQueryItem> BuildBillsQuery(DateTime? createdDate, string? name, string? billNo, long? mobileNo, int? customerType, char? isEstimate, int role)
        {
            // Base: Estimates (not converted to invoice) UNION Invoices
            var estimates = from b in _context.FwBills
                            where b.IsEstimateInvoice == "E"
                               && !_context.FwInvoices.Any(i => i.BillId == b.BillId && i.IsEstimateInvoice == "I")
                            select new
                            {
                                BillId = (long)b.BillId,
                                b.BillNo,
                                 TotalBillAmount = b.TotalBillAmount.HasValue && b.TotalBillAmount.Value != 0m
                                     ? b.TotalBillAmount
                                     : (decimal?)b.BillAmount,
                                BillDate = b.CreatedDate,
                                IsEstimateInvoice = b.IsEstimateInvoice,
                                CustomerType = b.CustomerType,
                                b.CustomerId,
                                WayBill = (string?)null,
                                InvoiceNo = (string?)null,
                                InvoiceStatus = 0,
                                IsInState = b.IsInState,
                                IsVerified = b.IsVerified,
                                DispatchStatus = b.DispatchStatus
                            };

            var invoices = from i in _context.FwInvoices
                           select new
                           {
                               BillId = (long)i.BillId,
                               i.BillNo,
                                TotalBillAmount = i.TotalBillAmount.HasValue && i.TotalBillAmount.Value != 0m
                                    ? i.TotalBillAmount
                                    : (decimal?)i.BillAmount,
                               BillDate = i.CreatedDate,
                               IsEstimateInvoice = i.IsEstimateInvoice,
                               CustomerType = i.CustomerType,
                               i.CustomerId,
                               WayBill = i.WayBill,
                               InvoiceNo = i.InvoiceNo,
                               InvoiceStatus = i.InvoiceStatus ?? 0,
                               IsInState = i.IsInState,
                               IsVerified = _context.FwBills.Where(bill => bill.BillId == i.BillId).Select(bill => bill.IsVerified).FirstOrDefault(),
                               DispatchStatus = "C"
                           };

            // Ensure both sequences are the same concrete anonymous type shape before Union
            var estimatesCast = estimates.Select(e => new { e.BillId, e.BillNo, e.TotalBillAmount, e.BillDate, e.IsEstimateInvoice, e.CustomerType, e.CustomerId, e.WayBill, e.InvoiceNo, e.InvoiceStatus, e.IsInState, e.IsVerified, e.DispatchStatus });
            var invoicesCast = invoices.Select(i => new { i.BillId, i.BillNo, i.TotalBillAmount, i.BillDate, i.IsEstimateInvoice, i.CustomerType, i.CustomerId, i.WayBill, i.InvoiceNo, i.InvoiceStatus, i.IsInState, i.IsVerified, i.DispatchStatus });
            var combined = estimatesCast.Concat(invoicesCast);

            var q = from b in combined
                    join c in _context.FwMasterCustomers on b.CustomerId equals c.CustomerId
                    join billRef in _context.FwBills on b.BillId equals billRef.BillId into billj
                    from billRefx in billj.DefaultIfEmpty()
                    where (!createdDate.HasValue || (b.BillDate != null && ((DateTime)b.BillDate).Date == createdDate.Value.Date))
                      && (string.IsNullOrEmpty(billNo) || b.BillNo == billNo)
                      && (!mobileNo.HasValue || (c.MobileNo ?? 0) == mobileNo.Value)
                      && (!customerType.HasValue || b.CustomerType == customerType.Value)
                      && (!isEstimate.HasValue || b.IsEstimateInvoice == isEstimate.Value.ToString())
                      && (string.IsNullOrEmpty(name) || c.Name.Contains(name))
                      && (role == 1 || (b.IsEstimateInvoice == "E" && (b.DispatchStatus == "D" || b.DispatchStatus == "P")))
                    select new BillsQueryItem
                    {
                        BillId = b.BillId,
                        BillNo = b.BillNo,
                        TotalBillAmount = b.TotalBillAmount,
                        BillDate = b.BillDate,
                        IsEstimateInvoice = b.IsEstimateInvoice,
                        CustomerType = b.CustomerType,
                        CustomerId = b.CustomerId,
                        WayBill = b.WayBill,
                        IsInState = b.IsInState,
                        DispatchStatus = billRefx != null ? billRefx.DispatchStatus : null,
                        OrderConfirmed = billRefx != null ? billRefx.IsVerified : b.IsVerified,
                        InvoiceNo = b.InvoiceNo,
                        InvoiceStatus = b.InvoiceStatus,
                        PhoneNo = c.MobileNo ?? 0,
                        CustomerName = c.Name,
                        CustAddress = c.CustAddress,
                        CustomerTypeText = b.CustomerType == 1 ? "Alpha" : b.CustomerType == 2 ? "Beta" : b.CustomerType == 3 ? "Gamma" : ""
                    };

            return q;
        }

        [HttpPost]
        public async Task<IActionResult> SetOrderConfirmation([FromBody] SetOrderConfirmationRequest request)
        {
            try
            {
                if (request == null || request.BillId <= 0)
                    return Json(new { success = false, message = "Invalid bill." });

                var roleValue = User.FindFirst(ClaimTypes.Role)?.Value;
                int.TryParse(roleValue, out var role);
                if (role != 1 && role != 2)
                    return Json(new { success = false, message = "Only administrators can update order confirmation." });

                var bill = await _context.FwBills.FirstOrDefaultAsync(x => x.BillId == request.BillId);
                if (bill == null)
                    return Json(new { success = false, message = "Bill not found." });

                bill.IsVerified = request.Confirmed;
                bill.VerifiedBy = int.TryParse(User.FindFirst("UserId")?.Value, out var userId) ? userId : null;
                bill.VerifiedDate = request.Confirmed ? DateTime.Now : null;
                await _context.SaveChangesAsync();
                return Json(new { success = true, confirmed = bill.IsVerified });
            }
            catch (Exception ex)
            {
                await _errorLogService.LogErrorAsync(User?.FindFirst("UserId")?.Value ?? "Unknown", "HomeController.SetOrderConfirmation", $"BillId={request?.BillId}", ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public sealed class SetOrderConfirmationRequest
        {
            public long BillId { get; set; }
            public bool Confirmed { get; set; }
        }
    }
}
