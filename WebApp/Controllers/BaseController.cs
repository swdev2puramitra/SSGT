using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebApp.Models;

namespace WebApp.Controllers
{
    /// <summary>
    /// Base controller for all application controllers.
    /// Provides common functionality like menu loading from session.
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Load menus from session (populated at login).
        /// This eliminates repeated database queries on every page navigation.
        /// Call this method in any action to load menus for the navbar.
        /// </summary>
        protected void LoadMenusFromSession()
        {
            try
            {
                var menuJson = HttpContext.Session.GetString("UserMenus");
                if (!string.IsNullOrEmpty(menuJson))
                {
                    var menus = JsonSerializer.Deserialize<List<MenuDTO>>(menuJson);
                    ViewBag.UserMenus = menus ?? new List<MenuDTO>();
                    return;
                }
                ViewBag.UserMenus = new List<MenuDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading menus from session: {ex.Message}");
                ViewBag.UserMenus = new List<MenuDTO>();
            }
        }

        /// <summary>
        /// Generate a document number (Bill/Invoice) using FwMasterBill logic.
        /// Returns the generated document number string and the next sequence number used.
        /// This method will persist updates to the master table via the provided context.
        /// </summary>
        protected (string docNo, int sequence) GenerateDocumentNumber(SsgtContext context, bool isEstimate)
        {
            try
            {
                string billTypeChar = isEstimate ? "E" : "I";
                int finYear = DateTime.Now.Year;
                int finMon = DateTime.Now.Month;
                if (finMon >= 1 && finMon <= 3) finYear = finYear - 1;

                var master = context.FwMasterBills.FirstOrDefault(m => m.FinYear == finYear && m.BillType == billTypeChar);
                int nextNumber = 1;
                if (master == null)
                {
                    master = new FwMasterBill { FinYear = finYear, BillType = billTypeChar, Bnumber = 1 };
                    context.FwMasterBills.Add(master);
                    context.SaveChanges();
                    nextNumber = 1;
                }
                else
                {
                    master.Bnumber = (master.Bnumber) + 1;
                    context.SaveChanges();
                    nextNumber = master.Bnumber;
                }

                var generated = string.Format("{0}{1:D4}", finYear, nextNumber);
                return (generated, nextNumber);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating document number: {ex.Message}");
                return (string.Empty, 0);
            }
        }

        /// <summary>
        /// Get item quantity description using database function FN_GET_ItemTypewiseDesc.
        /// This method fetches the description for a quantity of a specific item.
        /// Used by Stock and Billing controllers to display item quantity details.
        /// NOTE: Derived controllers must inject SsgtContext to enable this method.
        /// </summary>
        protected string GetItemStockDescriptionRequest(SsgtContext context, int itemId, int quantity)
        {
            try
            {
                if (itemId <= 0) return string.Empty;

                // This connection is owned by EF Core's DbContext. Do not dispose it here;
                // disposing the context-owned connection can clear its connection string
                // and break subsequent EF queries in the same request.
                var conn = context.Database.GetDbConnection();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT dbo.FN_GET_ItemTypewiseDesc(@ItemId, @Quantity)";
                var p1 = new SqlParameter("@ItemId", System.Data.SqlDbType.Int) { Value = itemId };
                var p2 = new SqlParameter("@Quantity", System.Data.SqlDbType.Int) { Value = quantity };
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                var wasClosed = conn.State == System.Data.ConnectionState.Closed;
                if (wasClosed) conn.Open();
                try
                {
                    var res = cmd.ExecuteScalar();
                    return res?.ToString() ?? string.Empty;
                }
                finally
                {
                    if (wasClosed && conn.State == System.Data.ConnectionState.Open)
                        conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching item stock desc: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// HTTP endpoint to get item quantity description.
        /// Accessible from AJAX calls and supports both Stock and Billing controllers.
        /// </summary>
        [HttpGet]
        public IActionResult GetItemStockDesc(int itemId, int quantity)
        {
            try
            {
                if (itemId <= 0) return Json(new { success = true, data = string.Empty });

                // Get SsgtContext from service provider (available in HTTP context)
                var dbContext = HttpContext.RequestServices.GetService(typeof(SsgtContext)) as SsgtContext;
                if (dbContext == null)
                    return Json(new { success = false, message = "Database context unavailable" });

                var desc = GetItemStockDescriptionRequest(dbContext, itemId, quantity);
                return Json(new { success = true, data = desc });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching item stock desc: {ex.Message}");
                return Json(new { success = false, message = "Error fetching item stock" });
            }
        }

        // NOTE: GenerateFinancialBillNo removed. Use GenerateDocumentNumber and format as needed (E/I + D4) to avoid duplication.
    }
}
