// Error Logging Implementation Summary
// 
// The following changes have been implemented to enable database-backed error logging:

// 1. NEW SERVICE INTERFACE: WebApp/Services/IErrorLogService.cs
//    - Provides contract for error logging to FW_ErrorLog table
//    - Two overloads: one for Exception objects, one for custom error messages
//    - Async methods for non-blocking logging

// 2. NEW SERVICE IMPLEMENTATION: WebApp/Services/ErrorLogService.cs
//    - Implements IErrorLogService
//    - Logs errors to FW_ErrorLog table with:
//      * UserId (extracted from user principal or defaults to "Unknown")
//      * MethodName (controller.action format)
//      * PrimaryKeyDetails (contextual info like billId, transferId, etc.)
//      * Error (full exception stack trace or custom message)
//      * ErrDate (current timestamp)
//    - Handles logging failures gracefully (writes to Debug output)

// 3. UPDATED: WebApp/Program.cs
//    - Added HttpContextAccessor registration (required for accessing User context)
//    - Registered IErrorLogService with DI container as Scoped service

// 4. UPDATED: WebApp/Controllers/BillingController.cs
//    - Injected IErrorLogService in constructor
//    - Updated catch blocks in the following methods to log errors:
//      * PreviewPdf - PDF generation errors
//      * GetCustomerByMobile - Customer lookup errors
//      * GetItemTypes - Item type retrieval errors
//      * GetItemTypePrice - Price lookup errors
//      * GetCustomerTypes - Customer type retrieval errors
//      * FireWorksBilling - Main billing save errors
//      * StockTransfer (GET) - Stock transfer load errors
//      * StockTransfer (POST) - Stock transfer save and transaction errors
//      * GetItems - Item search errors

// HOW IT WORKS:
// When an exception occurs in any catch block, the error is logged to FW_ErrorLog with:
// - Current user ID from the auth principal
// - Method name for traceability
// - Contextual details (e.g., billId, transferId, godownIds)
// - Complete exception information including stack trace

// EXAMPLE USAGE (already implemented):
//
// catch (Exception ex)
// {
//     var userId = User?.FindFirst("UserId")?.Value ?? User?.Identity?.Name ?? "Unknown";
//     await _errorLogService.LogErrorAsync(
//         userId,
//         "BillingController.PreviewPdf",
//         $"BillId={billId}, Template={template}",
//         ex);
//     return StatusCode(500);
// }

// QUERIES TO REVIEW LOGGED ERRORS:
// 1. Get all recent errors:
//    SELECT * FROM FW_ErrorLog ORDER BY ErrDate DESC
//
// 2. Get errors for a specific user:
//    SELECT * FROM FW_ErrorLog WHERE UserId = 'userid' ORDER BY ErrDate DESC
//
// 3. Get errors for a specific method:
//    SELECT * FROM FW_ErrorLog WHERE Methodname LIKE '%PreviewPdf%' ORDER BY ErrDate DESC
//
// 4. Get errors with specific context (e.g., stock transfer):
//    SELECT * FROM FW_ErrorLog WHERE PrimaryKeyDetails LIKE '%StockTransfer%' ORDER BY ErrDate DESC

// BENEFITS:
// ✓ Centralized error logging to database
// ✓ Full exception stack traces preserved
// ✓ User context captured for audit trail
// ✓ Method names enable root cause analysis
// ✓ Primary key details aid in reproduction
// ✓ Timestamps enable tracking over time
// ✓ Graceful handling (logging failures don't crash application)
// ✓ Scoped DI lifetime = new logger context per request
