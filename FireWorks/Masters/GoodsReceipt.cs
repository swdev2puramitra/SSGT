using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace FireWorks.Masters
{
    public partial class GoodsReceipt : Form
    {
        Int64? GoodsReceiptId = null;
        public GoodsReceipt()
        {
            InitializeComponent();
            CommonFunc.CompaniesFill(cbStockFrom);
            CommonFunc.FillGodwon(cbGodown);

            dtpArrive.MaxDate = DateTime.Now;
            dtpArrive.Value = dtpArrive.MaxDate;
        }
        public GoodsReceipt(Int64 StockId)
        {
            InitializeComponent();
            GoodsReceiptId = StockId;
            CommonFunc.CompaniesFill(cbStockFrom);
            CommonFunc.FillGodwon(cbGodown);
            LoadDetails(StockId);
        }

        private void LoadDetails(Int64 StockId)
        {
            CommonDal common = new CommonDal();
            GoodsReceiptEntity entity = common.GetStockDetails(StockId);
            if (entity == null)
            {
                CommonFunc.DisplayErrorMessage("Unable to get the Stock details");
                return;
            }
            cbGodown.SelectedValue = entity.GodownId;
            cbStockFrom.Text = entity.StockFrom;
            txtInvoice.Text = entity.InvoiceNo;
            txtComents.Text = entity.Comments;
            dtpArrive.Value = entity.ArrivalDate == null ? DateTime.Now : Convert.ToDateTime(entity.ArrivalDate);
            int i = 1;
            foreach (StockItemsList stockItems in entity.stockItems)
            {
                dgvItemsList.Rows.Add(new string[] {
                    i++.ToString(),stockItems.ItemCode.ToString(),stockItems.ItemName,stockItems.Detailed,stockItems.ItemTypeName,
                    stockItems.Quantity.ToString(),stockItems.ItemId.ToString()
                });
            }
        }

        private void fillCombo()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[2] {
                    new DataColumn("ItemTypeId", typeof(int)),
                    new DataColumn("TypeName", typeof(string))
                });
            dataTable.Rows.Add(null, "--Select Item Type--");
            cbItemType.DataSource = dataTable;
            cbItemType.DisplayMember = "TypeName";
            cbItemType.ValueMember = "ItemTypeId";
        }

        private void txtItemCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                using (ItemSearch frm = new ItemSearch(txtItemCode.Text, 0, false))
                {
                    if (!frm.Result)
                        frm.ShowDialog();

                    if (frm.Result)
                    {
                        ItemsEntity response = frm.items;
                        txtItemCode.Text = response.ItemCode;
                        txtItemName.Text = response.ItemName;
                        CommonFunc.ItemTypeByItem(cbItemType, Convert.ToInt32(response.ItemId));
                        cbItemType.SelectedValue = response.ItemType;
                        cbItemType.Enabled = true;
                        txtItemId.Text = response.ItemId.ToString();
                        txtQuantity.Focus();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbGodown.SelectedIndex == 0)
            {
                CommonFunc.DisplayErrorMessage("Select Godown");
                cbStockFrom.Focus();
                return;
            }
            if (cbStockFrom.SelectedIndex == 0)
            {
                CommonFunc.DisplayErrorMessage("Enter Stock From");
                cbStockFrom.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtInvoice.Text))
            {
                CommonFunc.DisplayErrorMessage("Enter Invoice No.");
                txtInvoice.Focus();
                return;
            }
            GoodsReceiptEntity goodsReceipts = new GoodsReceiptEntity();

            goodsReceipts.stockItems = new List<StockItemsList>();
            foreach (DataGridViewRow row in dgvItemsList.Rows)
            {
                StockItemsList receipt = new StockItemsList()
                {
                    ItemId = Convert.ToInt32(row.Cells["ItemId"].Value),
                    Quantity = Convert.ToInt64(row.Cells["Quantity"].Value)
                };

                goodsReceipts.stockItems.Add(receipt);
            }

            if (goodsReceipts.stockItems.Count == 0)
            {
                CommonFunc.DisplayErrorMessage("Add atleast one item");
                txtItemCode.Focus();
                return;
            }

            goodsReceipts.GodownId = Convert.ToInt32(cbGodown.SelectedValue);
            goodsReceipts.StockFrom = cbStockFrom.Text.ToString().Trim();
            goodsReceipts.InvoiceNo = txtInvoice.Text.Trim();
            goodsReceipts.ArrivalDate = Convert.ToDateTime(dtpArrive.Value);
            goodsReceipts.Comments = txtComents.Text.Trim();
            if (GoodsReceiptId != null)
            {
                goodsReceipts.StockId = Convert.ToInt32(GoodsReceiptId);
                goodsReceipts.DMLType = 'U';
            }
            else
            {
                goodsReceipts.DMLType = 'I';
            }
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            BillDal billDal = new BillDal();
            billDal.SaveStock(goodsReceipts, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                ResetForm(true);
                CommonFunc.SaveMessage(alertMsgInfo.Message);
            }
            else
            {
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetForm(true);
        }

        private void ResetForm(bool isGrid)
        {
            txtItemCode.Text = String.Empty;
            txtItemName.Text = String.Empty;
            txtItemId.Text = String.Empty;
            txtQuantity.Text = String.Empty;
            if (isGrid)
            {
                dgvItemsList.Rows.Clear();
                GoodsReceiptId = null;
                txtInvoice.Text = String.Empty;
                txtComents.Text = String.Empty;

                dtpArrive.MaxDate = DateTime.Now;
                dtpArrive.Value = dtpArrive.MaxDate;
                cbStockFrom.SelectedIndex = 0;
            }
            fillCombo();
            txtItemCode.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtItemId.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Item");
                txtItemCode.Focus();
                return;
            }
            if (String.IsNullOrEmpty(cbItemType.SelectedValue.ToString().Trim()))
            {
                CommonFunc.DisplayErrorMessage("Select Item Type");
                cbItemType.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter quantity");
                txtQuantity.Focus();
                return;
            }

            int selectedRowId = -1;
            foreach (DataGridViewRow viewRow in dgvItemsList.Rows)
            {
                if (viewRow.Cells["ItemId"].Value.ToString() == txtItemId.Text)
                {
                    selectedRowId = Convert.ToInt32(viewRow.Cells["SlNo"].Value.ToString()) - 1;
                    break;
                }
            }

            CommonDal common = new CommonDal();
            DataTable dataTable = common.GetItemPriceWithLowestLevel(Convert.ToInt32(txtItemId.Text),
                Convert.ToInt32(cbItemType.SelectedValue), 0);
            int rows = dgvItemsList.Rows.Count + 1;
            int qty = Convert.ToInt32(txtQuantity.Text) * Convert.ToInt32(dataTable.Rows[0][2]);

            if (selectedRowId != -1)
            {
                qty = qty + Convert.ToInt32(dgvItemsList.Rows[selectedRowId].Cells["Quantity"].Value);
                dgvItemsList.Rows[selectedRowId].Cells["Quantity"].Value = qty.ToString();
                dgvItemsList.Rows[selectedRowId].Cells["Detailed"].Value = CommonFunc.GetItemTypeDesc(Convert.ToInt32(txtItemId.Text), qty);
            }
            else
            {
                /*
                 SlNo,ItemCode,ItemName,ItemTypeName,Quantity,ItemId
                 */

                dgvItemsList.Rows.Add(new string[] {
                    rows.ToString(),txtItemCode.Text,txtItemName.Text,CommonFunc.GetItemTypeDesc(Convert.ToInt32(txtItemId.Text), qty),dataTable.Rows[1][5].ToString(),
                    qty.ToString(),txtItemId.Text
                });
            }
            ResetForm(false);
        }

        private void dgvItemsList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                DialogResult result = MessageBox.Show("Are you sure to delete Item : "
                    + dgvItemsList.Rows[e.RowIndex].Cells["ItemName"].Value.ToString(),
                    CommonFunc.ApplicationTitile, MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    dgvItemsList.Rows.Remove(dgvItemsList.Rows[e.RowIndex]);
                }
                int i = 1;
                foreach (DataGridViewRow row in dgvItemsList.Rows)
                {
                    row.Cells["SlNo"].Value = i.ToString();
                    i++;
                }
            }
        }
    }
}
