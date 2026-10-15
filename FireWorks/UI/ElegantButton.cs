using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FireWorks.UI
{
    public class ElegantButton : Button
    {
        public bool Primary { get; set; } = false;

        public ElegantButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = new Font("Segoe UI", 9, FontStyle.Regular);
            ForeColor = Color.Black;
            BackColor = Color.WhiteSmoke;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Rounded rectangle path
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 6;
                Rectangle rect = ClientRectangle;
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                Region = new Region(path);

                // Background fill
                Color baseColor = Primary ? Color.FromArgb(0, 120, 215) : BackColor;
                Color hoverColor = Primary ? Color.FromArgb(0, 150, 255) : Color.Gainsboro;

                Color fillColor = _hovering ? hoverColor : baseColor;
                using (SolidBrush brush = new SolidBrush(fillColor))
                {
                    pevent.Graphics.FillPath(brush, path);
                }

                // Border
                using (Pen pen = new Pen(Color.LightGray, 1))
                {
                    pevent.Graphics.DrawPath(pen, path);
                }

                // Text
                TextRenderer.DrawText(pevent.Graphics, Text, Font, rect,
                    Primary ? Color.White : ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private bool _hovering = false;
        protected override void OnMouseEnter(EventArgs e)
        {
            _hovering = true;
            Invalidate();
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            _hovering = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
    }
}
