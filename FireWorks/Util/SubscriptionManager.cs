using System;

namespace FireWorks.Util
{
    public static class SubscriptionManager
    {
        private const string EnvVarName = "APP_EXPIRY_DATE";

        // Called once at startup
        public static void Initialize()
        {
            string expiryValue = Environment.GetEnvironmentVariable(EnvVarName, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(expiryValue))
            {
                // First time → add default expiry (15 days ahead)
                string defaultExpiry = DateTime.Now.AddDays(15).ToString("yyyy-MM-dd");
                Environment.SetEnvironmentVariable(EnvVarName, defaultExpiry, EnvironmentVariableTarget.Machine);
                Console.WriteLine("Created subscription expiry: " + defaultExpiry);
            }
        }

        public static DateTime GetExpiryDate()
        {
            string expiryValue = Environment.GetEnvironmentVariable(EnvVarName, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(expiryValue))
                return DateTime.Now; // default if missing

            if (expiryValue.Equals("PERMANENT", StringComparison.OrdinalIgnoreCase))
                return DateTime.MaxValue; // treat permanent as max date

            if (DateTime.TryParse(expiryValue, out DateTime expiryDate))
                return expiryDate;

            return DateTime.Now; // fallback
        }

        public static bool IsValid()
        {
            DateTime expiryDate = GetExpiryDate();
            if (expiryDate == DateTime.MaxValue) return true; // permanent
            return DateTime.Now <= expiryDate;
        }

        public static void Renew(DateTime newExpiry)
        {
            string newExpiryValue = newExpiry.ToString("yyyy-MM-dd");
            Environment.SetEnvironmentVariable(EnvVarName, newExpiryValue, EnvironmentVariableTarget.Machine);
        }

        public static void ActivatePermanent()
        {
            Environment.SetEnvironmentVariable(EnvVarName, "PERMANENT", EnvironmentVariableTarget.Machine);
        }
      
    }
}
