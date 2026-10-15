using System;
using System.Drawing;
using System.Windows.Forms;
using FireWorks.UI;

namespace FireWorks.UI.Controls
{
    public class RightSidebarControl : UserControl
    {
        public event EventHandler ExportPdfClicked;
        public event EventHandler ExportExcelClicked;
        public event EventHandler PrintClicked;

        private Button btnExportExcel;
        private Button btnExportPdf;
        private Button btnPrint;
        private Panel summaryPanel;
        // summary labels exposed for updates
        private Label lblSummaryTitle;
        private Label lblSummaryLines;
        private int summaryMaxHeight;

        public RightSidebarControl()
        {
            Initialize();
            // default maximum height for the summary area
            SummaryMaxHeight = 360;
        }

        private void Initialize()
        {
            // narrower sidebar to leave more room for grid
            this.Width = 180;
            this.Dock = DockStyle.Right;
            this.Padding = new Padding(8);

            var container = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            // Summary placeholder
            summaryPanel = new Panel
            {
                BackColor = UiTheme.PanelBackground,
                Width = this.Width - 16,
                // increase summary height to better display multiple lines
                Height = 180,
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 10)
            };
            lblSummaryTitle = new Label { Text = "Summary", Font = new Font(UiTheme.AppFont.FontFamily, 10F, FontStyle.Bold), Dock = DockStyle.Top };
            // use AutoSize and Dock Top so we can measure preferred size and grow/shrink dynamically
            lblSummaryLines = new Label { Text = "Total Items\n0", AutoSize = true, Dock = DockStyle.Top, TextAlign = ContentAlignment.TopLeft };
            summaryPanel.Controls.Add(lblSummaryLines);
            summaryPanel.Controls.Add(lblSummaryTitle);

            // adjust height dynamically when panel or content changes
            summaryPanel.SizeChanged += (s, e) => UpdateSummaryHeight();

            // Actions panel
            var actionsPanel = new Panel { BackColor = UiTheme.PanelBackground, Width = this.Width - 12, Height = 120, Padding = new Padding(8), Margin = new Padding(0, 0, 0, 8) };

            int btnW = Math.Max(80, actionsPanel.Width - 20);
            btnExportExcel = new Button { Text = "Export to Excel", Width = btnW, Height = 34, Margin = new Padding(0, 0, 0, 8) };
            btnExportPdf = new Button { Text = "Export to PDF", Width = btnW, Height = 34, Margin = new Padding(0, 0, 0, 8) };
            btnPrint = new Button { Text = "Print", Width = btnW, Height = 34 };

            // apply consistent button style
            try
            {
                UiHelpers.ApplyElegantStyle(btnExportExcel, false);
                UiHelpers.ApplyElegantStyle(btnExportPdf, false);
                UiHelpers.ApplyElegantStyle(btnPrint, false);
            }
            catch { }

            btnExportExcel.Click += (s, e) => ExportExcelClicked?.Invoke(this, EventArgs.Empty);
            btnExportPdf.Click += (s, e) => ExportPdfClicked?.Invoke(this, EventArgs.Empty);
            btnPrint.Click += (s, e) => PrintClicked?.Invoke(this, EventArgs.Empty);

            actionsPanel.Controls.Add(btnExportExcel);
            actionsPanel.Controls.Add(btnExportPdf);
            actionsPanel.Controls.Add(btnPrint);

            // place actions first so Export/Print buttons appear at the top of the right sidebar
            //container.Controls.Add(actionsPanel);
            container.Controls.Add(summaryPanel);

            this.Controls.Add(container);
        }

        /// <summary>
        /// Update summary text lines shown in the sidebar. Each string will be placed on its own line.
        /// </summary>
        public void SetSummaryLines(string[] lines)
        {
            try
            {
                if (lines == null || lines.Length == 0)
                {
                    lblSummaryLines.Text = "";
                }
                else
                {
                    lblSummaryLines.Text = string.Join("\n", lines);
                }
                UpdateSummaryHeight();
            }
            catch { }
        }

        /// <summary>
        /// Convenience to set a single multi-line summary text.
        /// </summary>
        public void SetSummaryText(string text)
        {
            try { lblSummaryLines.Text = text ?? string.Empty; } catch { }
            UpdateSummaryHeight();
        }

        /// <summary>
        /// Maximum height the summary panel will expand to. If the content requires more height than this
        /// the panel will be capped and scrollbars from the container will be used.
        /// </summary>
        public int SummaryMaxHeight
        {
            get => summaryMaxHeight;
            set
            {
                summaryMaxHeight = Math.Max(80, value);
                UpdateSummaryHeight();
            }
        }

        private void UpdateSummaryHeight()
        {
            try
            {
                if (summaryPanel == null || lblSummaryLines == null || lblSummaryTitle == null)
                    return;

                // determine available width for the label
                int availableWidth = Math.Max(20, summaryPanel.ClientSize.Width - summaryPanel.Padding.Horizontal - 10);
                lblSummaryLines.MaximumSize = new Size(availableWidth, 0);

                int linesHeight = lblSummaryLines.PreferredSize.Height;
                int titleHeight = lblSummaryTitle.PreferredSize.Height;
                int total = titleHeight + linesHeight + summaryPanel.Padding.Vertical + 8; // small extra spacing

                // enforce minimum and maximum
                int min = 60;
                total = Math.Max(min, total);
                total = Math.Min(SummaryMaxHeight, total) + 20;

                summaryPanel.Height = total;
            }
            catch { }
        }
    }
}
