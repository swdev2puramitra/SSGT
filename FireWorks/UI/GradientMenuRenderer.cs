using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FireWorks.UI
{
    internal class GradientMenuRenderer : ToolStripProfessionalRenderer
    {
        private readonly Color topColor;
        private readonly Color bottomColor;

        public GradientMenuRenderer(Color top, Color bottom) : base()
        {
            topColor = top;
            bottomColor = bottom;
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            try
            {
                var rect = e.AffectedBounds;
                using (var brush = new LinearGradientBrush(rect, topColor, bottomColor, 90f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
            catch { base.OnRenderToolStripBackground(e); }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            try
            {
                var item = e.Item as ToolStripItem;
                var rc = new Rectangle(Point.Empty, item.Size);
                Color back = Color.FromArgb(180, Color.White);
                if (e.Item.Selected || e.Item.Pressed)
                {
                    back = UiTheme.Focus;
                }
                using (var brush = new SolidBrush(back))
                {
                    e.Graphics.FillRectangle(brush, rc);
                }
            }
            catch { base.OnRenderMenuItemBackground(e); }
        }
    }
}
