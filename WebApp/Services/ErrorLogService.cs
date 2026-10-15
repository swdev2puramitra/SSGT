using WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly SsgtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ErrorLogService(SsgtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogErrorAsync(string userId, string methodName, string? primaryKeyDetails, Exception? exception)
        {
            var errorMessage = exception == null
                ? "Unknown error"
                : $"{exception.GetType().Name}: {exception.Message}. StackTrace: {exception.StackTrace}";

            var safeErrorMessage = errorMessage.Length > 2000
                ? errorMessage[..2000]
                : errorMessage;
            await LogErrorAsync(userId, methodName, primaryKeyDetails, safeErrorMessage);
        }

        public async Task LogErrorAsync(string userId, string methodName, string? primaryKeyDetails, string errorMessage)
        {
            try
            {
                // Check if the context is not disposed before proceeding
                if (_context?.Database.CanConnect() == false)
                {
                    System.Diagnostics.Debug.WriteLine("Database context is not available for error logging");
                    return;
                }

                var errorLog = new FwErrorLog
                {
                    UserId = userId ?? "Unknown",
                    Methodname = methodName,
                    PrimaryKeyDetails = primaryKeyDetails,
                    Error = errorMessage,
                    ErrDate = DateTime.Now
                };

                _context.FwErrorLogs.Add(errorLog);
                await _context.SaveChangesAsync();
            }
            catch (ObjectDisposedException)
            {
                // Context was disposed by DI container before async operation completed
                // This is an indication that LogErrorAsync was not awaited
                System.Diagnostics.Debug.WriteLine($"DbContext disposed before error logging. Ensure LogErrorAsync is awaited in catch blocks. Error: {errorMessage}");
            }
            catch (Exception logException)
            {
                // Avoid logging errors in the error logger itself
                System.Diagnostics.Debug.WriteLine($"Failed to log error to database: {errorMessage}. LogException: {logException.Message}");
            }
        }
    }
}
