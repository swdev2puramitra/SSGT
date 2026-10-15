using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FireWorks.UI;

namespace FireWorks.UI.Controls
{
    public class HeaderControl : UserControl
    {
        private Label lblTitle;
        public HeaderControl()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Height = 64;
            this.Dock = DockStyle.Top;
            // use transparent background and paint a gradient in OnPaint for a modern look
            this.BackColor = Color.Transparent;

            lblTitle = new Label();
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = UiTheme.HeaderFont;
            lblTitle.Text = "";

            this.Controls.Add(lblTitle);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            try
            {
                var rect = this.ClientRectangle;
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, UiTheme.Primary, UiTheme.Secondary, 90f))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                // subtle bottom divider
                using (var p = new Pen(Color.FromArgb(80, Color.Black)))
                {
                    e.Graphics.DrawLine(p, 0, rect.Height - 1, rect.Width, rect.Height - 1);
                }
            }
            catch { }
        }

        [Category("Appearance")]
        public string Title
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }
    }
}
