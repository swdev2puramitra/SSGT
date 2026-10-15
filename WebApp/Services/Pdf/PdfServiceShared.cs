using QuestPDF.Fluent;

// Use fully-qualified System.Drawing types to avoid name collisions with QuestPDF

namespace WebApp.Services.Pdf
{
    internal static class PdfServiceShared
    {
        // Resize large images to a safe maximum dimension and return a temporary file path
        public static string PrepareImage(string sourcePath, int targetWidth = 800, int targetHeight = 800)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return sourcePath;

            try
            {
                using (var img = System.Drawing.Image.FromFile(sourcePath))
                {
                    var imgWidth = img.Size.Width;
                    var imgHeight = img.Size.Height;

                    // If image already fits the target box exactly, return original
                    if (imgWidth == targetWidth && imgHeight == targetHeight)
                        return sourcePath;

                    // Compute scale to fit into target box while preserving aspect ratio
                    var ratio = Math.Min((double)targetWidth / imgWidth, (double)targetHeight / imgHeight);
                    var newW = Math.Max(1, (int)Math.Round(imgWidth * ratio));
                    var newH = Math.Max(1, (int)Math.Round(imgHeight * ratio));

                    using (var bmp = new System.Drawing.Bitmap(targetWidth, targetHeight))
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        // fill with transparent / white background
                        g.Clear(System.Drawing.Color.White);
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        // center the scaled image inside the target box
                        var x = (targetWidth - newW) / 2;
                        var y = (targetHeight - newH) / 2;
                        g.DrawImage(img, x, y, newW, newH);

                        var temp = Path.Combine(Path.GetTempPath(), "pdf_img_" + Guid.NewGuid().ToString() + ".png");
                        bmp.Save(temp, System.Drawing.Imaging.ImageFormat.Png);
                        return temp;
                    }
                }
            }
            catch
            {
                return sourcePath; // fallback to original if resizing fails
            }
        }

        // Standard table cell style used across all PDF templates
        // Ensures every table cell has a visible border and consistent padding/color
        public static QuestPDF.Infrastructure.IContainer TableCellStyle(QuestPDF.Infrastructure.IContainer c)
        {
            return c.Border(0.5f).Padding(4).BorderColor(QuestPDF.Helpers.Colors.Grey.Medium);
        }
    }
}
