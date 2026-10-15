using FireWorks.Util;
using FireWorksDal.DataLayer;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FireWorks.Admin
{
    public partial class Customers : Form
    {
        DataView View = null;
        public Customers()
        {
            InitializeComponent();
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[2] {
                    new DataColumn("RowId", typeof(int)),
                    new DataColumn("Type", typeof(string))
                });
            dataTable.Rows.Add(null, "--Select Customer Type--");
            dataTable.Rows.Add(1, "Wholesale");
            dataTable.Rows.Add(2, "Semi Wholesale");
            dataTable.Rows.Add(3, "Retail");

            cbCusType.DataSource = dataTable;
            cbCusType.DisplayMember = "Type";
            cbCusType.ValueMember = "RowId";
            cbCusType.SelectedIndex = 0;


            FillGrid();
            DisplayGrid(dgvCustomer);
        }

        private void FillGrid()
        {
            CommonDal common = new CommonDal();

            View = new DataView(common.GetCustomerList());
            updateDGVData();
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            //objGrid.ReadOnly = true;
            CommonFunc.FormatGridLook(objGrid, false, false, false);

            objGrid.AutoGenerateColumns = false;

            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 60;
            objGrid.Columns["SlNo"].ReadOnly = true;
            objGrid.Columns["SlNo"].DisplayIndex = 0;

            objGrid.Columns["CustomerName"].HeaderText = "Name";
            objGrid.Columns["CustomerName"].Width = 200;
            objGrid.Columns["CustomerName"].ReadOnly = true;
            objGrid.Columns["CustomerName"].DisplayIndex = 1;

            objGrid.Columns["Address"].HeaderText = "Address";
            objGrid.Columns["Address"].Width = 240;
            objGrid.Columns["Address"].ReadOnly = true;
            objGrid.Columns["Address"].DisplayIndex = 2;

            objGrid.Columns["Mobile"].HeaderText = "Mobile";
            objGrid.Columns["Mobile"].Width = 100;
            objGrid.Columns["Mobile"].ReadOnly = true;
            objGrid.Columns["Mobile"].DisplayIndex = 3;

            objGrid.Columns["CustomerType"].HeaderText = "Customer Type";
            objGrid.Columns["CustomerType"].Width = 100;
            objGrid.Columns["CustomerType"].ReadOnly = true;
            objGrid.Columns["CustomerType"].DisplayIndex = 4;
        }

        private string FilterQuery()
        {
            StringBuilder Filter = new StringBuilder();
            Filter.Append("1=1");
            if (txtName.Text.Trim().Length > 0)
            {
                Filter.Append(" and [CustomerName] LIKE '*" + txtName.Text + "*'");
            }
            if (txtMobile.Text.Trim().Length == 10)
            {
                Filter.Append(" and [Mobile] = " + txtMobile.Text);
            }
            if (cbCusType.SelectedValue != null)
                if (cbCusType.SelectedValue.ToString().Trim().Length > 0)
                {
                    Filter.Append(" and [CustomerTypeId] = " + cbCusType.SelectedValue);
                }
            return Filter.ToString();
        }

        private void updateDGVData()
        {
            if (View != null)
            {
                View.RowFilter = FilterQuery();

                dgvCustomer.DataSource = null;
                dgvCustomer.DataSource = View;
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            updateDGVData();
        }

        private void cbCusType_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateDGVData();
        }
    }
}
