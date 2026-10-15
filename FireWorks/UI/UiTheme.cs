using System.Drawing;
using System.Windows.Forms;

namespace FireWorks.UI
{
    internal static class UiTheme
    {
        // Colors
        // Gradient colors tuned to match application header (lighter top -> darker bottom)
        public static readonly Color Primary = ColorTranslator.FromHtml("#2B8FE6"); // lighter blue
        public static readonly Color Secondary = ColorTranslator.FromHtml("#0D47A1"); // darker blue
        public static readonly Color Background = ColorTranslator.FromHtml("#F5F7FA");
        public static readonly Color PanelBackground = Color.White;
        public static readonly Color PrimaryText = ColorTranslator.FromHtml("#1F2937");
        public static readonly Color SecondaryText = ColorTranslator.FromHtml("#6B7280");
        public static readonly Color Border = ColorTranslator.FromHtml("#D9DEE7");
        public static readonly Color Success = ColorTranslator.FromHtml("#2E7D32");
        public static readonly Color Warning = ColorTranslator.FromHtml("#ED8B00");
        public static readonly Color Danger = ColorTranslator.FromHtml("#D32F2F");
        public static readonly Color Info = ColorTranslator.FromHtml("#0288D1");

        // Layout / spacing - calculated based on screen DPI for better scaling
        private static int? _calculatedControlHeight = null;

        public static int ControlHeight
        {
            get
            {
                if (!_calculatedControlHeight.HasValue)
                {
                    _calculatedControlHeight = CalculateControlHeight();
                }
                return _calculatedControlHeight.Value;
            }
        }

        public static readonly int ControlSpacing = 8;
        public static readonly Padding ControlPadding = new Padding(6);

        // Focus / accent
        public static readonly Color Focus = ColorTranslator.FromHtml("#1976D2");

        // Typography
        // Increased textbox font from 10pt to 11pt for better readability
        public static readonly Font AppFont = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font GridFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        public static readonly Font HeaderFont = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);

        /// <summary>
        /// Calculate appropriate control height based on screen DPI and resolution.
        /// Uses a base height of 32px for standard DPI (96) and scales proportionally.
        /// Minimum height is 32px, maximum is 48px to ensure usability.
        /// </summary>
        private static int CalculateControlHeight()
        {
            try
            {
                // Get current graphics for DPI detection
                using (Graphics g = Graphics.FromHwnd(System.IntPtr.Zero))
                {
                    float dpiX = g.DpiX;
                    float dpiY = g.DpiY;

                    // Standard DPI is 96, calculate scaling factor
                    float scaleFactor = dpiY / 96f;

                    // Base height increased from 30 to 32 for better visibility
                    int baseHeight = 32;

                    // Calculate scaled height
                    int scaledHeight = (int)(baseHeight * scaleFactor);

                    // Clamp between 32 and 48 pixels for usability
                    int finalHeight = System.Math.Max(32, System.Math.Min(48, scaledHeight));

                    return finalHeight;
                }
            }
            catch
            {
                // Fallback to increased default if calculation fails
                return 32;
            }
        }
    }
}
