using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Data;
using System.Windows.Forms;

namespace FireWorks.Admin
{
    public partial class StockTrade : Form
    {
        public StockTrade()
        {
            InitializeComponent();

            dtpFrom.Format = DateTimePickerFormat.Custom;
            dtpFrom.CustomFormat = "dd-MMM-yyyy";

            dtpTo.Format = DateTimePickerFormat.Custom;
            dtpTo.CustomFormat = "dd-MMM-yyyy";

            dtpFrom.MaxDate = DateTime.Now;
            dtpFrom.Value = DateTime.Now;
            dtpTo.MaxDate = DateTime.Now;
            dtpTo.Value = DateTime.Now;

            CommonFunc.fillCustomer(cbCustType);
            cbCustType.SelectedIndex = 0;

            DisplayGrid(dgvStock);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            //objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, true, false);

            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 60;
            objGrid.Columns["SlNo"].ReadOnly = true;

            objGrid.Columns["BillNo"].HeaderText = "Bill No";
            objGrid.Columns["BillNo"].Width = 80;
            objGrid.Columns["BillNo"].ReadOnly = true;

            objGrid.Columns["CustomerName"].HeaderText = "Name";
            objGrid.Columns["CustomerName"].Width = 200;
            objGrid.Columns["CustomerName"].ReadOnly = true;

            objGrid.Columns["TotalBillAmount"].HeaderText = "Bill Amount";
            objGrid.Columns["TotalBillAmount"].Width = 100;
            objGrid.Columns["TotalBillAmount"].ReadOnly = true;

        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            dgvStock.Rows.Clear();
            if (String.IsNullOrEmpty(dtpFrom.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Select From date");
                dtpFrom.Focus();
                return;
            }

            if (String.IsNullOrEmpty(dtpTo.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Select To date");
                dtpTo.Focus();
                return;
            }

            int? CustomerType = null;
            if (!String.IsNullOrEmpty(cbCustType.SelectedValue.ToString()))
                CustomerType = Convert.ToInt32(cbCustType.SelectedValue);
            BillDal dal = new BillDal();
            DataSet dataSet = dal.GetStockTrade(Convert.ToDateTime(dtpFrom.Value), Convert.ToDateTime(dtpTo.Value),
                CustomerType);

            if (dataSet != null)
            {
                decimal total = 0;
                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    dgvStock.Rows.Add(new string[] { dr["SlNo"].ToString(), dr["BillNo"].ToString(), dr["Name"].ToString(),
                    dr["BillDate"].ToString(),dr["TotalBillAmount"].ToString()});
                    total += Convert.ToDecimal(dr["TotalBillAmount"]);
                }
                lblTotal.Text = Math.Round(total, 2).ToString();
                DisplayGrid(dgvStock);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Now;
            dtpTo.Value = DateTime.Now;
            lblTotal.Text = "0.00";
            cbCustType.SelectedIndex = 0;
        }

        private void btnEstimate_Click(object sender, EventArgs e)
        {
            EstimateBills billing = new EstimateBills()
            {
                MdiParent = this.MdiParent
            };
            this.Close();
            billing.Show();
        }
    }
}
