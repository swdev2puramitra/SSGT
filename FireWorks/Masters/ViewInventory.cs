using CrystalDecisions.Shared;
using FireWorks.Reports;
using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using FireWorksEntity.Masters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FireWorks.UI;

namespace FireWorks.Masters
{
    public partial class ViewInventory : Form
    {
        DataView View = null;
        bool IsModified = false, IsPageLoad = false;

        // Paging fields
        private int pageSize = 15;
        private int currentPage = 1;
        private int totalPages = 1;
        // Shared font to avoid per-row allocation
        private static readonly System.Drawing.Font InventoryFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        public ViewInventory()
        {
            InitializeComponent();

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int w = Width >= screen.Width ? screen.Width : (screen.Width + Width) / 2;
            int h = Height >= screen.Height ? screen.Height : (screen.Height + Height) / 2;
            this.Location = new Point((screen.Width - w) / 2, (screen.Height - h) / 2);
            this.Size = new Size(w, h);
            WindowState = FormWindowState.Maximized;

            FillGrid();
            DisplayGrid(dgvInventory);
            bindCombo();

            // Wire up filter event handlers
            try
            {
                cbCategory.SelectedIndexChanged += cbCategory_SelectedIndexChanged;
                cbSubCategory.SelectedIndexChanged += cbSubCategory_SelectedIndexChanged;
            }
            catch { }

            // Apply global UI styling to paging controls
            try
            {
                if (cbPageSize != null)
                {
                    UiHelpers.ApplyComboBoxStyle(cbPageSize);
                }
                if (btnFirst != null)
                {
                    btnFirst.BackColor = UiTheme.Primary;
                    btnFirst.ForeColor = System.Drawing.Color.White;
                    btnFirst.Font = new System.Drawing.Font(UiTheme.AppFont.FontFamily, 10F, System.Drawing.FontStyle.Bold);
                    btnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnFirst.FlatAppearance.BorderColor = UiTheme.Secondary;
                    btnFirst.FlatAppearance.BorderSize = 1;
                }
                if (btnPrev != null)
                {
                    btnPrev.BackColor = UiTheme.Primary;
                    btnPrev.ForeColor = System.Drawing.Color.White;
                    btnPrev.Font = new System.Drawing.Font(UiTheme.AppFont.FontFamily, 10F, System.Drawing.FontStyle.Bold);
                    btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnPrev.FlatAppearance.BorderColor = UiTheme.Secondary;
                    btnPrev.FlatAppearance.BorderSize = 1;
                }
                if (btnNext != null)
                {
                    btnNext.BackColor = UiTheme.Primary;
                    btnNext.ForeColor = System.Drawing.Color.White;
                    btnNext.Font = new System.Drawing.Font(UiTheme.AppFont.FontFamily, 10F, System.Drawing.FontStyle.Bold);
                    btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnNext.FlatAppearance.BorderColor = UiTheme.Secondary;
                    btnNext.FlatAppearance.BorderSize = 1;
                }
                if (btnLast != null)
                {
                    btnLast.BackColor = UiTheme.Primary;
                    btnLast.ForeColor = System.Drawing.Color.White;
                    btnLast.Font = new System.Drawing.Font(UiTheme.AppFont.FontFamily, 10F, System.Drawing.FontStyle.Bold);
                    btnLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    btnLast.FlatAppearance.BorderColor = UiTheme.Secondary;
                    btnLast.FlatAppearance.BorderSize = 1;
                }
                if (lblPageInfo != null)
                {
                    lblPageInfo.Font = new System.Drawing.Font(UiTheme.AppFont.FontFamily, 10F, System.Drawing.FontStyle.Bold);
                    lblPageInfo.ForeColor = UiTheme.PrimaryText;
                }
                if (panelPaging != null)
                {
                    panelPaging.BackColor = UiTheme.PanelBackground;
                    panelPaging.BorderStyle = BorderStyle.FixedSingle;
                }

            }
            catch { }

            // default page size
            try
            {
                if (cbPageSize != null && cbPageSize.Items.Count > 0)
                    cbPageSize.SelectedIndex = 0;
            }
            catch { }

            IsPageLoad = true;
            if (UserSessionEntity.RoleId == 1)
                btnUpdate.Visible = true;
            else
                btnUpdate.Visible = false;

            // Initialize the right sidebar and layout
            try
            {
                StabilizeLayout();
            }
            catch { }
        }

        private void StabilizeLayout()
        {
            try
            {
                // ensure panelPaging is docked bottom and visible
                try
                {
                    var pp = this.Controls["panelPaging"] as Panel;
                    if (pp != null)
                    {
                        pp.Dock = DockStyle.Bottom;
                        pp.BringToFront();
                    }
                }
                catch { }

                // Add rightSidebar directly to the form (not in contentPanel)
                // This avoids layout conflicts with existing filter controls
                FireWorks.UI.Controls.RightSidebarControl sidebar = null;
                var sb = this.Controls.Find("rightSidebar", true);
                if (sb != null && sb.Length > 0)
                    sidebar = sb[0] as FireWorks.UI.Controls.RightSidebarControl;

                if (sidebar == null)
                {
                    try
                    {
                        // Position sidebar on the right side of the form, below filter controls
                        // Header = 38px (0-38), Filter controls = 49-73, so start sidebar at Y=85
                        sidebar = new FireWorks.UI.Controls.RightSidebarControl()
                        {
                            Name = "rightSidebar",
                            Width = 200,
                            Location = new Point(this.ClientSize.Width - 190, 140),
                            Height = this.ClientSize.Height - 85 - 48, // Subtract header, filter space, and paging panel height
                            Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
                        };
                        this.Controls.Add(sidebar);
                        sidebar.BringToFront();
                    }
                    catch { }
                }

                // Adjust dgvInventory to the right of grid and left of sidebar
                if (dgvInventory != null && dgvInventory.Parent == this)
                {
                    // Calculate grid width: available width minus sidebar width
                    int availableWidth = this.ClientSize.Width - 180; // 180 for sidebar
                    if (dgvInventory.Width != availableWidth - 12)
                    {
                        dgvInventory.Size = new Size(availableWidth - 12, dgvInventory.Height);
                    }
                }
            }
            catch { }
        }


        private void bindCombo()
        {
            CommonFunc.CategoryFill(cbCategory);
            CommonFunc.SubCategoryFill(cbSubCategory, null, true);
            CommonFunc.CompaniesFill(cbCompany);

            txtItemName.Focus();
        }
        private void FillGrid()
        {
            CommonDal common = new CommonDal();
            // fetch first page from database using server-side paging
            bool allRows = false;
            if (cbPageSize != null && cbPageSize.SelectedItem != null && cbPageSize.SelectedItem.ToString().Equals("All", StringComparison.OrdinalIgnoreCase))
                allRows = true;

            // Get filter values
            string itemNameOrCode = string.IsNullOrWhiteSpace(txtItemName.Text) ? null : txtItemName.Text.Trim();
            int? categoryId = null;
            int? subCategoryId = null;
            int? companyId = null;

            if (cbCategory.SelectedValue != null && int.TryParse(cbCategory.SelectedValue.ToString(), out int catId) && catId > 0)
                categoryId = catId;

            if (cbSubCategory.SelectedValue != null && int.TryParse(cbSubCategory.SelectedValue.ToString(), out int subCatId) && subCatId > 0)
                subCategoryId = subCatId;

            if (cbCompany.SelectedValue != null && int.TryParse(cbCompany.SelectedValue.ToString(), out int cmpId) && cmpId > 0)
                companyId = cmpId;

            View = new DataView(common.GetItemsList(true, currentPage, pageSize, allRows, itemNameOrCode, categoryId, subCategoryId, companyId));
            updateDGVData();
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            //objGrid.ReadOnly = true;
            CommonFunc.FormatGridLook(objGrid, false, false, false);

            // apply shared font for better performance and consistent scaling
            try
            {
                objGrid.DefaultCellStyle.Font = InventoryFont;
            }
            catch { }

            objGrid.ColumnHeadersHeight = 60;
            objGrid.AutoGenerateColumns = false;

            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 40;
            objGrid.Columns["SlNo"].ReadOnly = true;
            objGrid.Columns["SlNo"].DisplayIndex = 1;

            objGrid.Columns["ItemCode"].HeaderText = "Item Code";
            objGrid.Columns["ItemCode"].Width = 70;
            objGrid.Columns["ItemCode"].ReadOnly = true;
            objGrid.Columns["ItemCode"].DisplayIndex = 2;

            objGrid.Columns["ItemName"].HeaderText = "Item Name";
            objGrid.Columns["ItemName"].Width = 240;
            objGrid.Columns["ItemName"].ReadOnly = true;
            objGrid.Columns["ItemName"].DisplayIndex = 3;

            objGrid.Columns["StockPosition"].HeaderText = "Stock Position";
            objGrid.Columns["StockPosition"].Width = 140;
            objGrid.Columns["StockPosition"].ReadOnly = true;
            objGrid.Columns["StockPosition"].DisplayIndex = 4;

            objGrid.Columns["WholeSalePrice"].HeaderText = "Whole Sale Price";
            objGrid.Columns["WholeSalePrice"].Width = 70;
            objGrid.Columns["WholeSalePrice"].ReadOnly = false;
            objGrid.Columns["WholeSalePrice"].DisplayIndex = 5;

            objGrid.Columns["SemiWholeSalePrice"].HeaderText = "Semi Whole Sale Price";
            objGrid.Columns["SemiWholeSalePrice"].Width = 80;
            objGrid.Columns["SemiWholeSalePrice"].ReadOnly = false;
            objGrid.Columns["SemiWholeSalePrice"].DisplayIndex = 6;

            objGrid.Columns["RetailPrice"].HeaderText = "Retail Price";
            objGrid.Columns["RetailPrice"].Width = 70;
            objGrid.Columns["RetailPrice"].ReadOnly = false;
            objGrid.Columns["RetailPrice"].DisplayIndex = 7;

            if (objGrid.Columns["CompanName"] != null)
            {
                objGrid.Columns["CompanName"].HeaderText = "Company Name";
                objGrid.Columns["CompanName"].Width = 90;
                objGrid.Columns["CompanName"].ReadOnly = true;
                objGrid.Columns["CompanName"].DisplayIndex = 8;
            }
            else
            {
                objGrid.Columns["CompanyName"].HeaderText = "Company Name";
                objGrid.Columns["CompanyName"].Width = 90;
                objGrid.Columns["CompanyName"].ReadOnly = true;
                objGrid.Columns["CompanyName"].DisplayIndex = 8;
            }
            objGrid.Columns["CatDescription"].HeaderText = "Category";
            objGrid.Columns["CatDescription"].Width = 90;
            objGrid.Columns["CatDescription"].ReadOnly = true;
            objGrid.Columns["CatDescription"].DisplayIndex = 9;

            objGrid.Columns["SubCatDescription"].HeaderText = "Sub Category";
            objGrid.Columns["SubCatDescription"].Width = 80;
            objGrid.Columns["SubCatDescription"].ReadOnly = true;
            objGrid.Columns["SubCatDescription"].DisplayIndex = 10;

            objGrid.Columns["ConversionName"].HeaderText = "Conversion Name";
            objGrid.Columns["ConversionName"].Width = 100;
            objGrid.Columns["ConversionName"].ReadOnly = true;
            objGrid.Columns["ConversionName"].DisplayIndex = 11;

            objGrid.Columns["Contains"].HeaderText = "Contains";
            objGrid.Columns["Contains"].Width = 80;
            objGrid.Columns["Contains"].ReadOnly = true;
            objGrid.Columns["Contains"].DisplayIndex = 12;

            objGrid.Columns["Column1"].HeaderText = "View";
            objGrid.Columns["Column1"].Width = 60;
            objGrid.Columns["Column1"].ReadOnly = true;
            // ensure view button is visible and positioned at end if necessary
            try
            {
                objGrid.Columns["Column1"].Visible = true;
                objGrid.Columns["Column1"].DisplayIndex = Math.Max(0, Math.Min(13, objGrid.Columns.Count - 1));
            }
            catch { }

            objGrid.Columns["IsUpdated"].HeaderText = "Is Updated";
            objGrid.Columns["IsUpdated"].Width = 100;
            objGrid.Columns["IsUpdated"].ReadOnly = true;
            objGrid.Columns["IsUpdated"].DisplayIndex = 14;

            string[] hideCols = new string[] { "ItemId", "IsUpdated", "CompanyId", "CategoryId", "SubCategoryID", "TotalRows", "TotalPages" };
            foreach (string hide in hideCols)
            {
                if (objGrid.Columns[hide] != null)
                    objGrid.Columns[hide].Visible = false;
            }
            if (cbWhole.Checked)
                dgvInventory.Columns["WholeSalePrice"].Visible = true;
            else
                dgvInventory.Columns["WholeSalePrice"].Visible = false;
        }

        private void dgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (IsModified)
            {
                DialogResult result;

                result = MessageBox.Show(this, "Few amounts are modified. Do you want to save?", CommonFunc.ApplicationTitile, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);
                if (result == DialogResult.Yes)
                {
                    return;
                }
            }
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                //TODO - Button Clicked - Execute Code Here
                int itemId = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["ItemId"].Value);
                Items items = new Items(itemId) { fromForm = true };
                items.MdiParent = this.MdiParent;
                this.Close();
                items.Show();
            }
        }

        private string FilterQuery()
        {
            StringBuilder Filter = new StringBuilder();
            Filter.Append("1=1");
            if (txtItemName.Text.Trim().Length > 0)
            {
                Filter.Append(" and ([ItemName] LIKE '*" + txtItemName.Text + "*' or [ItemCode] LIKE '*" + txtItemName.Text + "*')");
            }
            if (cbCategory.SelectedValue != null)
                if (cbCategory.SelectedValue.ToString().Trim().Length > 0)
                {
                    Filter.Append(" and [CategoryId] = " + cbCategory.SelectedValue);
                }
            if (cbSubCategory.SelectedValue != null)
                if (cbSubCategory.SelectedValue.ToString().Trim().Length > 0)
                {
                    Filter.Append(" and [SubCategoryID] = " + cbSubCategory.SelectedValue);
                }
            if (cbCompany.SelectedValue != null)
                if (cbCompany.SelectedValue.ToString().Trim().Length > 0)
                {
                    if (cbCompany.SelectedValue.ToString().Trim() != "System.Data.DataRowView")
                        Filter.Append(" and [CompanyId] = " + cbCompany.SelectedValue);
                }
            return Filter.ToString();
        }

        private void txtItemName_TextChanged(object sender, EventArgs e)
        {
            updateDGVData();
        }

        private void cbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            updateDGVData();
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            CommonFunc.SubCategoryFill(cbSubCategory, cbCategory.SelectedValue as int?, false);
            updateDGVData();
        }

        private void cbSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            updateDGVData();
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear all filter controls
                txtItemName.Text = string.Empty;

                if (cbCategory.Items.Count > 0)
                    cbCategory.SelectedIndex = -1;

                if (cbSubCategory.Items.Count > 0)
                    cbSubCategory.SelectedIndex = -1;

                if (cbCompany.Items.Count > 0)
                    cbCompany.SelectedIndex = -1;

                // Reset to first page and refresh
                currentPage = 1;
                updateDGVData();
            }
            catch { }
        }

        private void updateDGVData()
        {
            // Use server-side paging: request current page from DB
            CommonDal common = new CommonDal();
            bool allRows = false;
            if (cbPageSize != null && cbPageSize.SelectedItem != null && cbPageSize.SelectedItem.ToString().Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                allRows = true;
                // when requesting all rows, ensure we fetch from page 1
                currentPage = 1;
            }

            // Get filter values
            string itemNameOrCode = string.IsNullOrWhiteSpace(txtItemName.Text) ? null : txtItemName.Text.Trim();
            int? categoryId = null;
            int? subCategoryId = null;
            int? companyId = null;

            if (cbCategory.SelectedValue != null && int.TryParse(cbCategory.SelectedValue.ToString(), out int catId) && catId > 0)
                categoryId = catId;

            if (cbSubCategory.SelectedValue != null && int.TryParse(cbSubCategory.SelectedValue.ToString(), out int subCatId) && subCatId > 0)
                subCategoryId = subCatId;

            if (cbCompany.SelectedValue != null && int.TryParse(cbCompany.SelectedValue.ToString(), out int cmpId) && cmpId > 0)
                companyId = cmpId;

            // Call overloaded method with filters
            DataTable pageTable = common.GetItemsList(true, currentPage, pageSize, allRows, itemNameOrCode, categoryId, subCategoryId, companyId);

            // Determine total records and total pages from returned table if provided
            int totalRecords = 0;
            int serverTotalPages = 0;
            if (pageTable != null && pageTable.Rows.Count > 0)
            {
                if (pageTable.Columns.Contains("TotalRows"))
                {
                    int.TryParse(pageTable.Rows[0]["TotalRows"].ToString(), out totalRecords);
                }
                if (pageTable.Columns.Contains("TotalPages"))
                {
                    int.TryParse(pageTable.Rows[0]["TotalPages"].ToString(), out serverTotalPages);
                }
            }

            if (totalRecords == 0)
            {
                // fallback to LastGetItemsTotalRecords or pageTable row count
                totalRecords = common.LastGetItemsTotalRecords > 0 ? common.LastGetItemsTotalRecords : (pageTable != null ? pageTable.Rows.Count : 0);
            }

            if (serverTotalPages > 0)
            {
                totalPages = serverTotalPages;
            }
            else
            {
                totalPages = totalRecords == 0 ? 1 : (int)Math.Ceiling(totalRecords / (double)pageSize);
            }

            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            // aggregate stock position for the returned page
            Hashtable hashtable = new Hashtable();
            if (pageTable != null)
            {
                foreach (DataRow dr in pageTable.Rows)
                {
                    if (pageTable.Columns.Contains("StockPosition") && dr["StockPosition"] != DBNull.Value)
                    {
                        string sp = dr["StockPosition"].ToString();
                        string[] parts = sp.Split(' ');
                        if (parts.Length > 1 && int.TryParse(parts[0], out int qty))
                        {
                            string unit = parts[1];
                            if (hashtable.Contains(unit))
                                hashtable[unit] = Convert.ToInt32(hashtable[unit]) + qty;
                            else
                                hashtable.Add(unit, qty);
                        }
                    }
                }
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DictionaryEntry de in hashtable)
            {
                stringBuilder.Append(de.Value + " " + de.Key);
                stringBuilder.AppendLine();
            }
            //lblData.Text = stringBuilder.ToString();

            // update right sidebar summary if present
            try
            {
                var arr = this.Controls.Find("rightSidebar", true);
                if (arr != null && arr.Length > 0 && arr[0] is FireWorks.UI.Controls.RightSidebarControl right)
                {
                    right.SetSummaryText(stringBuilder.ToString());
                }
            }
            catch { }

            dgvInventory.DataSource = pageTable;

            // set serial numbers relative to full filtered set (uses server total/offset)
            int startSerial = (currentPage - 1) * pageSize + 1;
            if (pageTable != null)
            {
                int serial = startSerial;
                foreach (DataGridViewRow row in dgvInventory.Rows)
                {
                    row.Cells["SlNo"].Value = (serial++).ToString();
                }
            }

            UpdatePageInfo(totalRecords);

            // enable/disable navigation buttons
            try
            {
                if (allRows)
                {
                    btnFirst.Enabled = false;
                    btnPrev.Enabled = false;
                    btnNext.Enabled = false;
                    btnLast.Enabled = false;
                }
                else
                {
                    btnFirst.Enabled = currentPage > 1;
                    btnPrev.Enabled = currentPage > 1;
                    btnNext.Enabled = currentPage < totalPages;
                    btnLast.Enabled = currentPage < totalPages;
                }
            }
            catch { }
        }

        private void UpdatePageInfo(int totalRecords)
        {
            lblPageInfo.Text = string.Format("Page {0} / {1} (Records: {2})", currentPage, totalPages, totalRecords);
        }

        private void cbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPageSize.SelectedItem == null) return;
            string val = cbPageSize.SelectedItem.ToString();
            if (int.TryParse(val, out int size))
            {
                pageSize = size;
                currentPage = 1;
                updateDGVData();
            }
            else if (val.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                // request all rows from server
                pageSize = 0;
                currentPage = 1;
                updateDGVData();
            }
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (currentPage != 1)
            {
                currentPage = 1;
                updateDGVData();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                updateDGVData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                updateDGVData();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (currentPage != totalPages)
            {
                currentPage = totalPages;
                updateDGVData();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            List<ItemsEntity> items = new List<ItemsEntity>();
            ItemsEntity itemsEntity = null;
            foreach (DataGridViewRow viewRow in dgvInventory.Rows)
            {
                if (viewRow.Cells["IsUpdated"].Value != null)
                    if (viewRow.Cells["IsUpdated"].Value.ToString() == "Y")
                    {
                        itemsEntity = new ItemsEntity();
                        itemsEntity.ItemId = Convert.ToInt32(viewRow.Cells["ItemId"].Value);
                        itemsEntity.WholeSalePrice = Convert.ToDecimal(viewRow.Cells["WholeSalePrice"].Value);
                        itemsEntity.SemiWholeSalePrice = Convert.ToDecimal(viewRow.Cells["SemiWholeSalePrice"].Value);
                        itemsEntity.RetailPrice = Convert.ToDecimal(viewRow.Cells["RetailPrice"].Value);

                        items.Add(itemsEntity);
                    }
            }
            if (items.Count == 0)
            {
                CommonFunc.DisplayErrorMessage("No rows are modified");
                return;
            }
            CommonDal dal = new CommonDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            dal.UpdateItemPrice(items, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                IsModified = false;
                CommonFunc.SaveMessage(alertMsgInfo.Message);
                FillGrid();
                DisplayGrid(dgvInventory);
            }
            else
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
        }

        private void dgvInventory_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsPageLoad)
            {
                var senderGrid = (DataGridView)sender;

                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn column &&
                    e.RowIndex >= 0)
                {
                    if (column.Name == "WholeSalePrice")
                    {
                        CommonDal common = new CommonDal();
                        CompanyEntity entity = common.GetCompanyMargin(Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["CompanyId"].Value));
                        decimal WholeSale = Convert.ToDecimal(dgvInventory.Rows[e.RowIndex].Cells["WholeSalePrice"].Value);
                        dgvInventory.Rows[e.RowIndex].Cells["SemiWholeSalePrice"].Value = Math.Round(WholeSale +
                            (WholeSale * entity.SemiWholeSale / 100), 0).ToString();
                        dgvInventory.Rows[e.RowIndex].Cells["RetailPrice"].Value = Math.Round(WholeSale +
                            (WholeSale * entity.Retail / 100), 0).ToString();
                    }
                    if (column.Name == "WholeSalePrice" ||
                        column.Name == "SemiWholeSalePrice" ||
                        column.Name == "RetailPrice")
                    {
                        dgvInventory.Rows[e.RowIndex].Cells["IsUpdated"].Value = "Y";
                        IsModified = true;
                    }
                }
            }
        }

        private void cbWhole_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWhole.Checked)
                dgvInventory.Columns["WholeSalePrice"].Visible = true;
            else
                dgvInventory.Columns["WholeSalePrice"].Visible = false;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dgvInventory.Columns)
            {
                dt.Columns.Add(col.Name);
            }

            foreach (DataGridViewRow row in dgvInventory.Rows)
            {
                DataRow dRow = dt.NewRow();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dRow[cell.ColumnIndex] = cell.Value;
                }
                dt.Rows.Add(dRow);
            }
            Reports.Inventory inventory = new Reports.Inventory();
            inventory.Database.Tables["USP_GET_ItemsList"].SetDataSource(dt);

            inventory.PrintOptions.PaperOrientation = PaperOrientation.Landscape;
            inventory.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            //inventory.PrintToPrinter(1, false, 0, 0);
            using (ViewReport report = new ViewReport(inventory))
            {
                report.Text = "Inventory";
                report.ShowDialog();
            }
        }

        private void dgvInventory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Only format the row being painted to avoid expensive per-row work on every paint
            if (e == null) return;
            if (e.RowIndex < 0 || e.RowIndex >= dgvInventory.Rows.Count) return;

            var row = dgvInventory.Rows[e.RowIndex];
            try
            {
                var cell = row.Cells["StockPosition"];
                if (cell?.Value != null)
                {
                    var parts = cell.Value.ToString().Split(' ');
                    if (parts.Length > 0 && int.TryParse(parts[0], out int qty))
                    {
                        if (qty >= 0 && qty <= 5)
                        {
                            row.DefaultCellStyle.ForeColor = Color.Orange;
                        }
                        else if (qty < 0)
                        {
                            row.DefaultCellStyle.ForeColor = Color.Red;
                        }
                        else
                        {
                            row.DefaultCellStyle.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch { }
        }
    }
}
