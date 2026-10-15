using FireWorks.Util;
using FireWorksDal.DataLayer;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class EstimateBills : Form
    {
        bool formload = false;
        public EstimateBills()
        {
            InitializeComponent();
            dtpBill.MaxDate = DateTime.Now;
            dtpBill.Value = dtpBill.MaxDate;
            //WindowState = FormWindowState.Maximized;
            //Height = 550;
            //dgvBills.Height = 500;
            CommonFunc.fillCustomer(cbCustomer);
            //fillGrid();
        }

        private void fillGrid()
        {
            Int64? PhoneNo = null, BillId = null;
            DateTime? CreateDate = null;
            int? CustomerType = null;
            string customer = null;

            if (!String.IsNullOrEmpty(txtPhone.Text.Trim()))
                PhoneNo = Convert.ToInt64(txtPhone.Text);
            if (!String.IsNullOrEmpty(txtCustomer.Text.Trim()))
                customer = txtCustomer.Text.Trim();
            if (!String.IsNullOrEmpty(txtBillId.Text.Trim()))
                BillId = Convert.ToInt64(txtBillId.Text);
            if (cbCustomer.SelectedValue != null)
                if (!String.IsNullOrEmpty(cbCustomer.SelectedValue.ToString()))
                    CustomerType = Convert.ToInt32(cbCustomer.SelectedValue);
            if (!String.IsNullOrEmpty(dtpBill.Text.Trim()))
                CreateDate = Convert.ToDateTime(dtpBill.Value);

            if (!formload)
            {
                CreateDate = DateTime.Now;
                formload = true;
            }
            BillDal dal = new BillDal();
            dgvBills.DataSource = dal.GetEstimateBills(PhoneNo, customer, CreateDate, BillId, CustomerType);
            DisplayGrid(dgvBills);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, false, false);
            objGrid.AutoGenerateColumns = false;
            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 40;
            objGrid.Columns["SlNo"].DisplayIndex = 0;
            objGrid.Columns["SlNo"].ReadOnly = true;

            objGrid.Columns["CustomerName"].HeaderText = "Customer Name";
            objGrid.Columns["CustomerName"].Width = 180;
            objGrid.Columns["CustomerName"].DisplayIndex = 1;
            objGrid.Columns["CustomerName"].ReadOnly = true;

            objGrid.Columns["CustAddress"].HeaderText = "Address";
            objGrid.Columns["CustAddress"].Width = 120;
            objGrid.Columns["CustAddress"].DisplayIndex = 2;
            objGrid.Columns["CustAddress"].ReadOnly = true;

            objGrid.Columns["CustomerType"].HeaderText = "Customer Type";
            objGrid.Columns["CustomerType"].Width = 70;
            objGrid.Columns["CustomerType"].DisplayIndex = 3;
            objGrid.Columns["CustomerType"].ReadOnly = true;

            objGrid.Columns["BillId"].HeaderText = "Bill Id";
            objGrid.Columns["BillId"].Width = 80;
            objGrid.Columns["BillId"].DisplayIndex = 4;
            objGrid.Columns["BillId"].ReadOnly = true;
            objGrid.Columns["BillId"].Visible = false;

            objGrid.Columns["BillNo"].HeaderText = "Bill No";
            objGrid.Columns["BillNo"].Width = 80;
            objGrid.Columns["BillNo"].DisplayIndex = 5;
            objGrid.Columns["BillNo"].ReadOnly = true;

            objGrid.Columns["InvoiceNo"].HeaderText = "Invoice No";
            objGrid.Columns["InvoiceNo"].Width = 60;
            objGrid.Columns["InvoiceNo"].DisplayIndex = 6;
            objGrid.Columns["InvoiceNo"].ReadOnly = true;

            objGrid.Columns["BillAmount"].HeaderText = "Bill Amount";
            objGrid.Columns["BillAmount"].Width = 80;
            objGrid.Columns["BillAmount"].DisplayIndex = 7;
            objGrid.Columns["BillAmount"].ReadOnly = true;

            objGrid.Columns["BillDate"].HeaderText = "Bill Date";
            objGrid.Columns["BillDate"].Width = 90;
            objGrid.Columns["BillDate"].DisplayIndex = 8;
            objGrid.Columns["BillDate"].ReadOnly = true;

            objGrid.Columns["PhoneNo"].HeaderText = "Phone No.";
            objGrid.Columns["PhoneNo"].Width = 85;
            objGrid.Columns["PhoneNo"].DisplayIndex = 9;
            objGrid.Columns["PhoneNo"].ReadOnly = true;

            objGrid.Columns["Estimate"].HeaderText = "Estimate?";
            objGrid.Columns["Estimate"].Width = 70;
            objGrid.Columns["Estimate"].DisplayIndex = 10;
            objGrid.Columns["Estimate"].ReadOnly = true;

            objGrid.Columns["WayBill"].HeaderText = "Way Bill";
            objGrid.Columns["WayBill"].Width = 70;
            objGrid.Columns["WayBill"].DisplayIndex = 11;
            objGrid.Columns["WayBill"].ReadOnly = true;

            objGrid.Columns["Print"].HeaderText = "Print";
            objGrid.Columns["Print"].Width = 70;
            objGrid.Columns["Print"].DisplayIndex = 12;
            objGrid.Columns["Print"].ReadOnly = true;

            //objGrid.Columns["Update"].HeaderText = "Way Bill";
            //objGrid.Columns["Update"].Width = 70;
            //objGrid.Columns["Update"].DisplayIndex = 15;
            //objGrid.Columns["Update"].ReadOnly = true;

            objGrid.Columns["IsInState"].HeaderText = "IsInState";
            objGrid.Columns["IsInState"].Width = 70;
            objGrid.Columns["IsInState"].DisplayIndex = 13;
            objGrid.Columns["IsInState"].ReadOnly = true;
            objGrid.Columns["IsInState"].Visible = false;
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            if (txtPhone.Text.Length == 10)
                fillGrid();
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtPhone.Text))
                e.Handled = true;
        }

        private void txtBillId_TextChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        private void txtBillId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtBillId.Text))
                e.Handled = true;
        }

        private void dtpBill_ValueChanged(object sender, EventArgs e)
        {
            dtpBill.Format = DateTimePickerFormat.Custom;
            dtpBill.CustomFormat = "dd-MMM-yyyy";
            fillGrid();
        }

        private void dtpBill_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
            {
                dtpBill.CustomFormat = " ";
                fillGrid();
            }
        }

        private void dgvBills_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn column &&
                e.RowIndex >= 0)
            {
                Int64 BillId = Convert.ToInt64(dgvBills.Rows[e.RowIndex].Cells["BillId"].Value.ToString().Replace("E", ""));
                switch (column.Text.Trim().ToLower())
                {
                    case "print":
                        Billing bill = new Billing();
                        if (dgvBills.Rows[e.RowIndex].Cells["CustomerType"].Value.ToString().ToLower() == "Alpha".ToLower()
                            || dgvBills.Rows[e.RowIndex].Cells["CustomerType"].Value.ToString().ToLower() == "Beta".ToLower())
                        {
                            bill.PrintEstimate(BillId, false);
                        }
                        else
                        {
                            bill.PrintRetail(BillId);
                        }
                        return;
                    default:
                        break;
                }
            }
        }

        private void rbEstimate_CheckedChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        private void cbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        private void dgvBills_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dgvBills.Rows)
                if (Convert.ToBoolean(Myrow.Cells["IsVerified1"].Value))
                {
                    Myrow.DefaultCellStyle.BackColor = Color.FromArgb(128, 255, 128);
                }
        }

    }
}