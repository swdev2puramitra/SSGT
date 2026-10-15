using WebApp.Models;

namespace WebApp.Services
{
    public interface IErrorLogService
    {
        /// <summary>
        /// Log an error to the FW_ErrorLog table with exception details
        /// </summary>
        Task LogErrorAsync(string userId, string methodName, string? primaryKeyDetails, Exception? exception);

        /// <summary>
        /// Log an error to the FW_ErrorLog table with custom message
        /// </summary>
        Task LogErrorAsync(string userId, string methodName, string? primaryKeyDetails, string errorMessage);
    }
}
