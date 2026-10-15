using FireWorks.Util;
using FireWorks.Util;
using FireWorksDal.DataLayer;
using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FireWorks.UI;

namespace FireWorks.Masters
{
    public partial class InactiveItems : Form
    {
        DataView View = null;
        // paging fields
        private int pageSize = 20;
        private int currentPage = 1;
        private int totalPages = 1;
        private static readonly System.Drawing.Font InventoryFont = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        public InactiveItems()
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

            View = new DataView(common.GetItemsList(false, currentPage, pageSize, allRows, itemNameOrCode, categoryId, subCategoryId, companyId));
            dgvInventory.DataSource = null;
            // apply shared font
            try { dgvInventory.DefaultCellStyle.Font = InventoryFont; } catch { }
            updateDGVData();
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            //objGrid.ReadOnly = true;
            CommonFunc.FormatGridLook(objGrid, false, false, false);

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
            objGrid.Columns["Column1"].DisplayIndex = 13;

            string[] hideCols = new string[] { "ItemId", "CompanyId", "CategoryId", "SubCategoryID", "TotalRows", "TotalPages" };
            foreach (string hide in hideCols)
            {
                if (objGrid.Columns[hide] != null)
                    objGrid.Columns[hide].Visible = false;
            }
            try { objGrid.DefaultCellStyle.Font = InventoryFont; } catch { }
        }

        private void dgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
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
                Filter.Append(" and [ItemName] LIKE '*" + txtItemName.Text + "*' ");
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
            DataTable pageTable = common.GetItemsList(false, currentPage, pageSize, allRows, itemNameOrCode, categoryId, subCategoryId, companyId);

            int totalRecords = 0;
            int serverTotalPages = 0;
            if (pageTable != null && pageTable.Rows.Count > 0)
            {
                if (pageTable.Columns.Contains("TotalRows"))
                    int.TryParse(pageTable.Rows[0]["TotalRows"].ToString(), out totalRecords);
                if (pageTable.Columns.Contains("TotalPages"))
                    int.TryParse(pageTable.Rows[0]["TotalPages"].ToString(), out serverTotalPages);
            }
            if (totalRecords == 0)
                totalRecords = common.LastGetItemsTotalRecords > 0 ? common.LastGetItemsTotalRecords : (pageTable != null ? pageTable.Rows.Count : 0);

            if (serverTotalPages > 0)
                totalPages = serverTotalPages;
            else
                totalPages = totalRecords == 0 ? 1 : (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (currentPage > totalPages) currentPage = totalPages;
            if (currentPage < 1) currentPage = 1;

            dgvInventory.DataSource = pageTable;

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
            try { lblPageInfo.Text = string.Format("Page {0} / {1} (Records: {2})", currentPage, totalPages, totalRecords); } catch { }
        }

        private void dgvInventory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e == null) return;
            if (e.RowIndex < 0 || e.RowIndex >= dgvInventory.Rows.Count) return;
            try
            {
                var row = dgvInventory.Rows[e.RowIndex];
                var cell = row.Cells["StockPosition"];
                if (cell?.Value != null && cell.Value.ToString().Contains("-"))
                {
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
            catch { }
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
    }
}
