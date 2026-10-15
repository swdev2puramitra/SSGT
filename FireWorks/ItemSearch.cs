using FireWorks.Reports;
using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class ItemSearch : Form
    {
        public ItemsEntity items;
        public bool Result = false, _isPrice = false;
        int custType;
        public ItemSearch(string Search, int CustomerType, bool isPrice)
        {
            InitializeComponent();
            _isPrice = isPrice;
            fillGrid(Search, CustomerType);
            custType = CustomerType;
            txtSearch.Text = Search;
        }

        private void fillGrid(string Search, int CustomerType)
        {
            CommonDal dal = new CommonDal();
            DataView View = new DataView(dal.GetItemsList(Search, CustomerType, _isPrice));
            dgvItemsList.DataSource = null;
            dgvItemsList.DataSource = View;
            if (View.Count == 1)
            {
                SelectItem(0);
                return;
            }
            DisplayGrid(dgvItemsList);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, false, false);

            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 60;
            objGrid.Columns["SlNo"].ReadOnly = true;

            objGrid.Columns["ItemCode"].HeaderText = "Item Code";
            objGrid.Columns["ItemCode"].Width = 120;
            objGrid.Columns["ItemCode"].ReadOnly = true;

            objGrid.Columns["ItemName"].HeaderText = "Item Name";
            objGrid.Columns["ItemName"].Width = 200;
            objGrid.Columns["ItemName"].ReadOnly = true;

            objGrid.Columns["TypeName"].HeaderText = "Item Type";
            objGrid.Columns["TypeName"].Width = 80;
            objGrid.Columns["TypeName"].ReadOnly = true;

            objGrid.Columns["ItemType"].HeaderText = "Item Type Id";
            objGrid.Columns["ItemType"].Width = 80;
            objGrid.Columns["ItemType"].Visible = false;

            objGrid.Columns["ItemPrice"].HeaderText = "Price";
            objGrid.Columns["ItemPrice"].Width = 100;
            objGrid.Columns["ItemPrice"].ReadOnly = true;

            objGrid.Columns["StockPosition"].HeaderText = "Stock Position";
            objGrid.Columns["StockPosition"].Width = 120;
            objGrid.Columns["StockPosition"].ReadOnly = true;

            objGrid.Columns["ItemId"].HeaderText = "Item Id";
            objGrid.Columns["ItemId"].ReadOnly = true;
            objGrid.Columns["ItemId"].Visible = false;

            objGrid.Columns["CGST"].HeaderText = "CGST";
            objGrid.Columns["CGST"].ReadOnly = true;
            objGrid.Columns["CGST"].Visible = false;

            objGrid.Columns["SGST"].HeaderText = "SGST";
            objGrid.Columns["SGST"].ReadOnly = true;
            objGrid.Columns["SGST"].Visible = false;

            objGrid.Columns["IGST"].HeaderText = "IGST";
            objGrid.Columns["IGST"].ReadOnly = true;
            objGrid.Columns["IGST"].Visible = false;

            if (UserSessionEntity.RoleId != 1)
            {
                string[] hideCols = new string[] { "StockPosition" };
                foreach (string hide in hideCols)
                {
                    if (objGrid.Columns[hide] != null)
                        objGrid.Columns[hide].Visible = false;
                }
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                fillGrid(txtSearch.Text.Trim(), custType);
            }
        }

        private void dgvItemsList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectItem(e.RowIndex);
            }
        }

        private void dgvItemsList_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(e.KeyCode.ToString());
            if (e.KeyValue == 13)
            {
                int CurrentRow = dgvItemsList.CurrentCell.RowIndex - 1;
                //MessageBox.Show(dgvItemsList.SelectedCells[1].Value.ToString());
                SelectItem(dgvItemsList.CurrentRow.Index);
                //SelectItem(CurrentRow);
            }
        }

        private void dgvItemsList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dgvItemsList.Rows)
            {
                Myrow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
                if (Myrow.Cells["StockPosition"].Value != null)
                //if (Myrow.Cells["StockPosition"].Value.ToString().Contains("-"))
                {
                    string[] details = Myrow.Cells["StockPosition"].Value.ToString().Split(' ');
                    if (Convert.ToInt32(details[0]) >= 0 && Convert.ToInt32(details[0]) <= 5)
                        Myrow.DefaultCellStyle.ForeColor = Color.Orange;
                    else if (Convert.ToInt32(details[0]) < 0)
                        Myrow.DefaultCellStyle.ForeColor = Color.Red;
                }
            }
        }

        private void SelectItem(int RowId)
        {
            string[] details = dgvItemsList.Rows[RowId].Cells["StockPosition"].Value.ToString().Split(' ');
            items = new ItemsEntity
            {
                ItemId = Convert.ToInt32(dgvItemsList.Rows[RowId].Cells["ItemId"].Value),
                ItemCode = dgvItemsList.Rows[RowId].Cells["ItemCode"].Value.ToString(),
                ItemName = dgvItemsList.Rows[RowId].Cells["ItemName"].Value.ToString(),
                TypeName = dgvItemsList.Rows[RowId].Cells["TypeName"].Value.ToString(),
                ItemType = Convert.ToInt32(dgvItemsList.Rows[RowId].Cells["ItemType"].Value),
                ItemPrice = Convert.ToDecimal(dgvItemsList.Rows[RowId].Cells["ItemPrice"].Value),
                CGST = Convert.ToDecimal(dgvItemsList.Rows[RowId].Cells["CGST"].Value),
                SGST = Convert.ToDecimal(dgvItemsList.Rows[RowId].Cells["SGST"].Value),
                IGST = Convert.ToDecimal(dgvItemsList.Rows[RowId].Cells["IGST"].Value),
                Stock = dgvItemsList.Rows[RowId].Cells["StockPosition"].Value.ToString()
            };
            Result = true;
            this.Close();
        }
    }
}
