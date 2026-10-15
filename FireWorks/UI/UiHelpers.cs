using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FireWorks.UI
{
    internal static class UiHelpers
    {
        public static void ApplyGridStyle(DataGridView dgv)
        {
            if (dgv == null) return;
            try
            {
                // Basic grid setup
                dgv.EnableHeadersVisualStyles = false;
                dgv.BackgroundColor = UiTheme.PanelBackground;
                dgv.BorderStyle = BorderStyle.None;
                dgv.GridColor = UiTheme.Border;
                dgv.RowHeadersVisible = false;
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgv.AllowUserToAddRows = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
                dgv.DefaultCellStyle.Font = UiTheme.GridFont;

                // Header styling
                dgv.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.Primary;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font(
                    UiTheme.GridFont.FontFamily,
                    UiTheme.GridFont.Size,
                    FontStyle.Bold
                );
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

                // Row styling
                dgv.RowTemplate.Height = 28;
                dgv.RowsDefaultCellStyle.BackColor = Color.White;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

                // Selection visuals
                dgv.DefaultCellStyle.SelectionBackColor = UiTheme.Primary;
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;

                // Hover effect handlers (only once)
                if (dgv.Tag == null || dgv.Tag.ToString() != "__styled_grid_handlers")
                {
                    dgv.CellMouseEnter += (s, e) =>
                    {
                        if (e.RowIndex >= 0)
                        {
                            var row = dgv.Rows[e.RowIndex];
                            if (!row.Selected)
                                row.DefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
                        }
                    };

                    dgv.CellMouseLeave += (s, e) =>
                    {
                        if (e.RowIndex >= 0)
                        {
                            var row = dgv.Rows[e.RowIndex];
                            if (!row.Selected)
                                row.DefaultCellStyle.BackColor = (row.Index % 2 == 0)
                                    ? Color.White
                                    : Color.FromArgb(250, 250, 250);
                        }
                    };

                    dgv.SelectionChanged += (s, e) =>
                    {
                        foreach (DataGridViewRow r in dgv.Rows)
                        {
                            if (r.Selected)
                            {
                                r.DefaultCellStyle.BackColor = dgv.DefaultCellStyle.SelectionBackColor;
                                r.DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.SelectionForeColor;
                            }
                            else
                            {
                                r.DefaultCellStyle.BackColor = (r.Index % 2 == 0)
                                    ? Color.White
                                    : Color.FromArgb(250, 250, 250);
                                r.DefaultCellStyle.ForeColor = dgv.DefaultCellStyle.ForeColor;
                            }
                        }
                    };

                    dgv.Tag = "__styled_grid_handlers";
                }

                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

                // Align numeric/button columns
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    string name = col.Name.ToLowerInvariant();
                    if (name.Contains("price") || name.Contains("amount") ||
                        name.Contains("qty") || name.Contains("quantity") ||
                        name.Contains("total") || name.Contains("whole") ||
                        name.Contains("retail") || name.Contains("cost"))
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else if (col is DataGridViewButtonColumn)
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    else
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    }
                }

                // Patch: clear selection AFTER grid is fully bound/rendered
                dgv.DataBindingComplete += (s, e) =>
                {
                    dgv.ClearSelection();
                };
                dgv.HandleCreated += (s, e) =>
                {
                    dgv.ClearSelection();
                };
            }
            catch { }
        }


        /// <summary>
        /// Apply form-wide styles by traversing the control tree. This is a helper to avoid
        /// calling individual Apply* methods for each control in a form's load.
        /// It will apply sensible defaults based on control type. Controls can opt out by
        /// setting Tag = "__no_style".
        /// </summary>
        public static void ApplyFormStyle(Control parent)
        {
            if (parent == null) return;
            try
            {
                foreach (Control ctl in parent.Controls)
                {
                    // skip controls opting out
                    try { if (ctl.Tag != null && ctl.Tag.ToString() == "__no_style") continue; } catch { }

                    switch (ctl)
                    {
                        case TextBox tb:
                            ApplyInputStyle(tb);
                            break;
                        case ComboBox cb:
                            ApplyComboBoxStyle(cb);
                            break;
                        case DataGridView dgv:
                            ApplyGridStyle(dgv);
                            break;
                        case Button btn:
                            // do not force primary styling; keep default visual for safety
                            //ApplyButtonStyle(btn, false);
                            ApplyElegantStyle(btn, false);
                            break;
                        case Label lbl:
                            ApplyLabelStyle(lbl);
                            break;
                        case CheckBox ch:
                            ApplyCheckBoxStyle(ch);
                            break;
                        case GroupBox gb:
                            // style the groupbox's text by setting Font and ForeColor
                            try { gb.Font = new Font(UiTheme.AppFont.FontFamily, 10F, FontStyle.Bold); gb.ForeColor = UiTheme.PrimaryText; } catch { }
                            break;
                    }

                    // Recurse into child controls
                    if (ctl.HasChildren)
                    {
                        ApplyFormStyle(ctl);
                    }
                }
            }
            catch { }
        }

        // Lightweight loading overlay helpers (adds/removes a semi-transparent panel over the form)
        public static void ShowLoading(Form owner, string text = "Loading...")
        {
            if (owner == null) return;
            try
            {
                var existing = owner.Controls.Find("__loadingOverlay", false);
                if (existing != null && existing.Length > 0) return;

                var overlay = new Panel
                {
                    Name = "__loadingOverlay",
                    BackColor = Color.FromArgb(160, Color.White),
                    Dock = DockStyle.Fill,
                    Visible = true
                };

                var box = new Panel
                {
                    Width = 260,
                    Height = 80,
                    BackColor = UiTheme.PanelBackground,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var lbl = new Label
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Fill,
                    Font = new Font(UiTheme.AppFont.FontFamily, 10F, FontStyle.Regular),
                    Text = " " + text
                };

                var progress = new ProgressBar
                {
                    Style = ProgressBarStyle.Marquee,
                    MarqueeAnimationSpeed = 30,
                    Height = 14,
                    Dock = DockStyle.Bottom
                };

                box.Controls.Add(lbl);
                box.Controls.Add(progress);
                box.Left = (owner.ClientSize.Width - box.Width) / 2;
                box.Top = (owner.ClientSize.Height - box.Height) / 2;
                box.Anchor = AnchorStyles.None;

                overlay.Controls.Add(box);
                owner.Controls.Add(overlay);
                overlay.BringToFront();
            }
            catch { }
        }

        public static void HideLoading(Form owner)
        {
            if (owner == null) return;
            try
            {
                var existing = owner.Controls.Find("__loadingOverlay", false);
                if (existing != null && existing.Length > 0)
                {
                    foreach (Control c in existing) owner.Controls.Remove(c);
                }
            }
            catch { }
        }

        public static void ApplyInputStyle(TextBox txt)
        {
            if (txt == null) return;
            try
            {
                // Basic font and colors
                txt.Font = UiTheme.AppFont;
                txt.BackColor = UiTheme.PanelBackground;
                txt.ForeColor = UiTheme.PrimaryText;

                // If this textbox was previously wrapped by an earlier version, unwrap it now.
                if (txt.Parent != null && txt.Parent is Panel parentPanel && parentPanel.Name != null && parentPanel.Name.StartsWith("__wrap_"))
                {
                    var wrapper = parentPanel;
                    var grandParent = wrapper.Parent;
                    if (grandParent != null)
                    {
                        int idx = grandParent.Controls.GetChildIndex(wrapper);
                        // remove textbox from wrapper
                        wrapper.Controls.Remove(txt);

                        // restore textbox properties
                        txt.BorderStyle = BorderStyle.None; // no border as requested
                        txt.Margin = wrapper.Margin;
                        txt.Anchor = wrapper.Anchor;

                        // add textbox back to grandParent at the same index
                        grandParent.Controls.Add(txt);
                        grandParent.Controls.SetChildIndex(txt, idx);
                    }
                    // finally remove wrapper
                    try { grandParent.Controls.Remove(wrapper); } catch { }
                }

                // Do not draw any border or shadow — plain textbox without border
                txt.BorderStyle = BorderStyle.None;

                // Ensure height is consistent for single-line inputs
                if (!txt.Multiline)
                {
                    txt.Height = UiTheme.ControlHeight;
                }

                // Add vertical margin to textboxes for better spacing from labels
                // This prevents the "attached" appearance when labels are positioned above
                if (txt.Margin.Top == 0)
                {
                    txt.Margin = new Padding(txt.Margin.Left, 3, txt.Margin.Right, txt.Margin.Bottom);
                }

                // Add focus background change to improve focus visibility with more prominent colors
                // Avoid adding handlers multiple times by marking Tag.
                try
                {
                    if (txt.Tag == null || txt.Tag.ToString() != "__styled_input_handlers")
                    {
                        txt.GotFocus += (s, e) =>
                        {
                            try
                            {
                                // More prominent focus color
                                txt.BackColor = Color.FromArgb(230, 245, 255);
                            }
                            catch { }
                        };
                        txt.LostFocus += (s, e) =>
                        {
                            try
                            {
                                txt.BackColor = UiTheme.PanelBackground;
                            }
                            catch { }
                        };

                        // mark that handlers were added
                        txt.Tag = "__styled_input_handlers";
                    }
                }
                catch { }
            }
            catch { }
        }

        public static void ApplyComboBoxStyle(ComboBox cb)
        {
            if (cb == null) return;
            try
            {
                cb.Font = UiTheme.AppFont;
                cb.BackColor = UiTheme.PanelBackground;
                cb.ForeColor = UiTheme.PrimaryText;
                cb.FlatStyle = FlatStyle.Flat;
                cb.Height = UiTheme.ControlHeight;
                cb.Padding = UiTheme.ControlPadding;
            }
            catch { }
        }

        public static void ApplyLabelStyle(Label lbl, bool sectionHeader = false)
        {
            if (lbl == null) return;
            try
            {
                // Check if this is a header label (docked to top, typically used as form title)
                bool isHeaderLabel = lbl.Dock == DockStyle.Top &&
                                    (lbl.TextAlign == ContentAlignment.MiddleCenter ||
                                     lbl.TextAlign == ContentAlignment.TopCenter);

                if (isHeaderLabel)
                {
                    // Apply header styling with gradient background
                    // Use 12pt font and 38px height for better fit without overlap
                    lbl.Font = new Font(UiTheme.AppFont.FontFamily, 12F, FontStyle.Bold);
                    lbl.ForeColor = Color.White;
                    lbl.BackColor = UiTheme.Primary;
                    // Reduced height to 38px to prevent overlap with form controls
                    lbl.Height = 38;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;

                    // Add paint event for gradient if not already added
                    if (lbl.Tag == null || lbl.Tag.ToString() != "__styled_header_label")
                    {
                        lbl.Paint += (s, e) =>
                        {
                            try
                            {
                                var rect = lbl.ClientRectangle;
                                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                                    rect, UiTheme.Primary, UiTheme.Secondary, 90f))
                                {
                                    e.Graphics.FillRectangle(brush, rect);
                                }

                                // Draw text centered
                                using (var sf = new StringFormat())
                                {
                                    sf.Alignment = StringAlignment.Center;
                                    sf.LineAlignment = StringAlignment.Center;
                                    e.Graphics.DrawString(lbl.Text, lbl.Font, Brushes.White, rect, sf);
                                }
                            }
                            catch { }
                        };
                        lbl.Tag = "__styled_header_label";
                    }
                }
                else if (sectionHeader)
                {
                    lbl.Font = new Font(UiTheme.AppFont.FontFamily, 10F, FontStyle.Bold);
                    lbl.ForeColor = UiTheme.PrimaryText;
                    // Add margins to section headers for better spacing
                    lbl.Margin = new Padding(0, 8, 0, 4);
                }
                else
                {
                    lbl.Font = UiTheme.AppFont;
                    lbl.ForeColor = UiTheme.PrimaryText;

                    // For non-header labels: ensure consistent sizing and spacing
                    // If label has AutoSize enabled, add padding on the right side
                    if (lbl.AutoSize)
                    {
                        // AutoSize labels get right padding via margin
                        // This creates visual space between label and adjacent controls
                        if (lbl.Margin.Right < 8)
                        {
                            lbl.Margin = new Padding(lbl.Margin.Left, lbl.Margin.Top, 8, lbl.Margin.Bottom);
                        }
                    }
                    else
                    {
                        // If manually sized, ensure minimum height for readability
                        if (lbl.Height < UiTheme.ControlHeight)
                        {
                            lbl.Height = UiTheme.ControlHeight;
                        }
                    }

                    // Add margins to regular labels to prevent "attached" appearance
                    // Keep existing margin if set, otherwise apply default spacing
                    if (lbl.Margin.Top == 0 && lbl.Margin.Bottom == 0)
                    {
                        lbl.Margin = new Padding(lbl.Margin.Left, 0, 8, 2);
                    }

                    // Mark this label as styled so we can process it later for position-based spacing
                    if (lbl.Tag == null || !lbl.Tag.ToString().Contains("__styled_label"))
                    {
                        lbl.Tag = "__styled_label_" + Guid.NewGuid().ToString();
                    }
                }
            }
            catch { }
        }

        public static void ApplyCheckBoxStyle(CheckBox cb)
        {
            if (cb == null) return;
            try
            {
                cb.Font = UiTheme.AppFont;
                cb.ForeColor = UiTheme.PrimaryText;
                cb.FlatStyle = FlatStyle.Flat;
                cb.BackColor = Color.Transparent;
            }
            catch { }
        }

        public static void ApplyStatusStripStyle(StatusStrip status)
        {
            if (status == null) return;
            try
            {
                status.BackColor = UiTheme.PanelBackground;
                status.ForeColor = UiTheme.SecondaryText;
                status.ShowItemToolTips = true;
                status.SizingGrip = false;
                foreach (ToolStripItem item in status.Items)
                {
                    item.ForeColor = UiTheme.SecondaryText;
                    item.Font = new Font(UiTheme.AppFont.FontFamily, 9F);
                }
            }
            catch { }
        }

        public static void ApplyHeaderStyle(Controls.HeaderControl header, string title = null)
        {
            if (header == null) return;
            try
            {
                header.Title = title ?? header.Title;
                header.Height = 64;
            }
            catch { }
        }

        public static void ApplyMenuStyle(MenuStrip menu)
        {
            if (menu == null) return;
            try
            {
                // apply a subtle gradient renderer (white -> primary blue)
                menu.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                menu.Renderer = new GradientMenuRenderer(Color.White, UiTheme.Primary);
                menu.BackColor = Color.Transparent;
                menu.ForeColor = UiTheme.PrimaryText;
                menu.Font = new Font(UiTheme.AppFont.FontFamily, 10F);
                menu.Padding = new Padding(6, 4, 6, 4);
                menu.ImageScalingSize = new Size(20, 20);
                menu.GripStyle = ToolStripGripStyle.Hidden;
                // style existing items
                foreach (ToolStripItem item in menu.Items)
                {
                    item.ForeColor = UiTheme.PrimaryText;
                    item.Font = menu.Font;
                    // ensure dropdowns use the same renderer
                    if (item is ToolStripMenuItem mi && mi.DropDown != null)
                    {
                        mi.DropDown.RenderMode = ToolStripRenderMode.ManagerRenderMode;
                        mi.DropDown.Renderer = menu.Renderer;
                    }
                }
            }
            catch { }
        }

        public static void ApplyElegantStyle(Button btn, bool primary = false)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.LightGray;
            btn.Font = new Font("Segoe UI", primary ? 10 : 9, primary ? FontStyle.Bold : FontStyle.Regular);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.UseVisualStyleBackColor = false;
            /*
            // Create region from a GraphicsPath using AddRectangle (avoids passing Rectangle[] to a constructor expecting PointF[])
            try
            {
                using (var path = new GraphicsPath())
                {
                    path.AddRectangle(new Rectangle(0, 0, btn.Width, btn.Height));
                    btn.Region = new Region(path);
                }
            }
            catch { }

            btn.Paint += (s, e) =>
            {
                using (var brush = new LinearGradientBrush(btn.ClientRectangle,
                    Color.LightBlue, Color.DarkBlue, 45f))
                {
                    e.Graphics.FillRectangle(brush, btn.ClientRectangle);
                }
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font,
                    btn.ClientRectangle, btn.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            */
            if (primary)
            {
                btn.BackColor = Color.FromArgb(0, 120, 215); // elegant blue
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = Color.WhiteSmoke;
                btn.ForeColor = Color.Black;
            }

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = primary ? Color.FromArgb(0, 150, 255) : Color.Gainsboro;
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = primary ? Color.FromArgb(0, 120, 215) : Color.WhiteSmoke;
            };
        }

        /// <summary>
        /// Creates a filter panel for inventory forms with standard controls and a Clear Filters button.
        /// Reduces code duplication across inventory management forms.
        /// </summary>
        /// <param name="parentForm">The form to add the panel to</param>
        /// <param name="filterControls">Array of filter controls to add to the panel</param>
        /// <param name="clearFiltersClickHandler">Event handler for the Clear Filters button</param>
        /// <param name="panelHeight">Optional height for the filter panel (default 48)</param>
        public static FlowLayoutPanel CreateInventoryFilterPanel(
            Form parentForm,
            Control[] filterControls,
            System.EventHandler clearFiltersClickHandler,
            int panelHeight = 48)
        {
            if (parentForm == null || filterControls == null)
                return null;

            try
            {
                var filterPanel = new FlowLayoutPanel()
                {
                    Name = "filterPanel",
                    Dock = DockStyle.Top,
                    Height = panelHeight,
                    AutoSize = false,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = true,
                    Padding = new Padding(6)
                };

                foreach (var control in filterControls)
                {
                    if (control == null) continue;
                    try
                    {
                        parentForm.Controls.Remove(control);
                        control.Margin = new Padding(6, 6, 6, 6);
                        control.UseWaitCursor = false;
                        filterPanel.Controls.Add(control);
                    }
                    catch { }
                }

                // Add Clear Filters button
                if (clearFiltersClickHandler != null)
                {
                    var btnClearFilters = new Button
                    {
                        Name = "btnClearFilters",
                        Text = "Clear Filters",
                        AutoSize = false,
                        Size = new System.Drawing.Size(100, 30),
                        Margin = new Padding(6, 6, 6, 6),
                        UseVisualStyleBackColor = true
                    };
                    btnClearFilters.Click += clearFiltersClickHandler;
                    ApplyElegantStyle(btnClearFilters, false);
                    filterPanel.Controls.Add(btnClearFilters);
                }

                parentForm.Controls.Add(filterPanel);
                parentForm.Controls.SetChildIndex(filterPanel, 1);

                return filterPanel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a header control to a form and hides the legacy label12 header if it exists.
        /// Reduces code duplication across forms.
        /// </summary>
        /// <param name="parentForm">The form to add the header to</param>
        /// <param name="title">The header title text</param>
        /// <param name="legacyHeaderLabel">Optional legacy label to hide (e.g., label12)</param>
        public static Controls.HeaderControl AddFormHeader(Form parentForm, string title, Label legacyHeaderLabel = null)
        {
            if (parentForm == null) return null;

            try
            {
                var header = new Controls.HeaderControl
                {
                    Name = "headerControl",
                    Title = title
                };

                parentForm.Controls.Add(header);
                parentForm.Controls.SetChildIndex(header, 0);

                // Hide legacy label header if provided
                if (legacyHeaderLabel != null)
                {
                    try { legacyHeaderLabel.Visible = false; } catch { }
                }

                return header;
            }
            catch
            {
                return null;
            }
        }
    }
}
