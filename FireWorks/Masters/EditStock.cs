using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FireWorks.Masters
{
    public partial class EditStock : Form
    {
        DataView View = null;
        public EditStock()
        {
            InitializeComponent();
            fillGrid();

            CommonFunc.CompaniesFill(cbStockFrom);
        }

        private void fillGrid()
        {
            CommonDal dal = new CommonDal();
            View = new DataView(dal.GetStockList());
            dgvStockList.DataSource = null;
            dgvStockList.DataSource = View;
            DisplayGrid(dgvStockList);
        }
        private string FilterQuery()
        {
            StringBuilder Filter = new StringBuilder();
            Filter.Append("1=1");
            if (txtInvoice.Text.Trim().Length > 0)
            {
                Filter.Append(" and [InvoiceNo] LIKE '*" + txtInvoice.Text + "*' ");
            }
            if (cbStockFrom.SelectedIndex > 0)
            {
                Filter.Append(" and [StockFrom] LIKE '*" + cbStockFrom.Text.ToString().Trim() + "*' ");
            }

            return Filter.ToString();
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            // Ensure no auto-generated columns will be added/modified later
            objGrid.AutoGenerateColumns = false;

            CommonFunc.FormatGridLook(objGrid, false, false, false);

            // ... (configure headers, widths, ReadOnly exactly as before) ...

            // Keep GodownId hidden but DO NOT set DisplayIndex on it
            if (objGrid.Columns.Contains("GodownId"))
            {
                objGrid.Columns["GodownId"].HeaderText = "GodownId";
                objGrid.Columns["GodownId"].Width = 50;
                objGrid.Columns["GodownId"].Visible = false;
            }

            // After all visible columns are configured, compute and set DisplayIndex
            int idx = 0;
            string[] order = new[] { "StockId", "StockFrom", "GodownDescription", "ReceivedDt", "CreatedDate", "InvoiceNo", "ArrivalDt", "Comments" };
            foreach (var col in order)
            {
                if (objGrid.Columns.Contains(col) && objGrid.Columns[col].Visible)
                    objGrid.Columns[col].DisplayIndex = idx++;
            }

            if (objGrid.Columns.Contains("Edit") && objGrid.Columns["Edit"].Visible)
                objGrid.Columns["Edit"].DisplayIndex = idx++;
            if (objGrid.Columns.Contains("Delete") && objGrid.Columns["Delete"].Visible)
                objGrid.Columns["Delete"].DisplayIndex = idx++;
        }

        private void dgvStockList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn column && e.RowIndex >= 0)
            {
                Int64 StockId = Convert.ToInt64(dgvStockList.Rows[e.RowIndex].Cells["StockId"].Value);
                switch (column.Text.Trim().ToLower())
                {
                    case "edit":
                        GoodsReceipt goodsReceipt = new GoodsReceipt(StockId);
                        goodsReceipt.MdiParent = this.MdiParent;
                        this.Close();
                        goodsReceipt.Show();
                        break;
                    case "delete":
                        BillDal bill = new BillDal();
                        AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
                        bill.DeleteStock(StockId, alertMsgInfo);
                        if (alertMsgInfo.SaveStatus)
                        {
                            CommonFunc.SaveMessage(alertMsgInfo.Message);
                            fillGrid();
                        }
                        else
                        {
                            CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
                        }
                        break;
                }
            }
        }

        private void txtInvoice_TextChanged(object sender, EventArgs e)
        {
            if (View != null)
            {
                View.RowFilter = FilterQuery();
                dgvStockList.DataSource = View;
            }
        }

        private void cbStockFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (View != null)
            {
                View.RowFilter = FilterQuery();
                dgvStockList.DataSource = View;
            }
        }
    }
}
