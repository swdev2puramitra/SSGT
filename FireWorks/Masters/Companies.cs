using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using FireWorksEntity.Masters;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FireWorks.Masters
{
    public partial class Companies : Form
    {
        DataView View = null;
        public Companies()
        {
            InitializeComponent();
            this.Text = CommonFunc.ApplicationTitile;
            fillGrid();
        }

        #region Methods
        private void fillGrid()
        {
            CommonDal dal = new CommonDal();
            View = new DataView(dal.GetCompanies(null));
            dgvSubCategory.DataSource = null;
            dgvSubCategory.DataSource = View;
            DisplayGrid(dgvSubCategory);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, false, false);

            objGrid.Columns["Id"].HeaderText = "Id";
            objGrid.Columns["Id"].Width = 50;
            objGrid.Columns["Id"].ReadOnly = true;

            objGrid.Columns["CompanyName"].HeaderText = "Company Name";
            objGrid.Columns["CompanyName"].Width = 180;
            objGrid.Columns["CompanyName"].ReadOnly = true;

            objGrid.Columns["SemiWholeSale"].HeaderText = "Semi Whole Sale";
            objGrid.Columns["SemiWholeSale"].Width = 70;
            objGrid.Columns["SemiWholeSale"].ReadOnly = true;
            objGrid.Columns["SemiWholeSale"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["Retail"].HeaderText = "Retail";
            objGrid.Columns["Retail"].Width = 70;
            objGrid.Columns["Retail"].ReadOnly = true;
            objGrid.Columns["Retail"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["IsActive"].HeaderText = "Is Active";
            objGrid.Columns["IsActive"].Width = 70;
            objGrid.Columns["IsActive"].ReadOnly = true;

            objGrid.ColumnHeadersHeight = 60;
        }

        private string FilterQuery()
        {
            StringBuilder Filter = new StringBuilder();
            Filter.Append("1=1");
            if (txtSearch.Text.Trim().Length > 0)
            {
                Filter.Append(" and [CompanyName] LIKE '*" + txtSearch.Text + "*' ");
            }

            return Filter.ToString();
        }
        #endregion

        #region Events
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCompName.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Company Name.");
                return;
            }
           
            if (String.IsNullOrEmpty(txtSemi.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Semi Whole Sale.");
                return;
            }
            
            if (String.IsNullOrEmpty(txtRetail.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Retail.");
                return;
            }

            if (!String.IsNullOrEmpty(txtSemi.Text.Trim()))
            {
                if (Convert.ToDecimal(txtSemi.Text) <= 0 || Convert.ToDecimal(txtSemi.Text) > 100)
                {
                    CommonFunc.DisplayErrorMessage("Invalid margin for Semi-Wholesale");
                    txtSemi.Focus();
                    return;
                }
            }
            if (!String.IsNullOrEmpty(txtRetail.Text.Trim()))
            {
                if (Convert.ToDecimal(txtRetail.Text) <= 0 || Convert.ToDecimal(txtRetail.Text) > 100)
                {
                    CommonFunc.DisplayErrorMessage("Invalid margin for Retail");
                    txtRetail.Focus();
                    return;
                }
            }
            if (!rbActive.Checked && !rbInactive.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Active / Inactive");
                return;
            }
            CommonDal dal = new CommonDal();
            CompanyEntity category = new CompanyEntity();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            category.CompanyName = txtCompName.Text.Trim();
            category.SemiWholeSale = Convert.ToDecimal(txtSemi.Text);
            category.Retail = Convert.ToDecimal(txtRetail.Text);
            category.IsActive = rbActive.Checked;
            if (!String.IsNullOrEmpty(lblCompId.Text.Trim()))
            {
                category.CompanyId = Convert.ToInt32(lblCompId.Text);
                category.DMLType = 'U';
            }
            else
                category.DMLType = 'I';
            dal.SaveCompanies(category, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                lblCompId.Text = string.Empty;
                txtCompName.Text = string.Empty;
                txtSemi.Text = string.Empty;
                txtRetail.Text = string.Empty;
                txtCompName.Focus();
                gbCompId.Visible = false;
                fillGrid();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
            }
            else
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CommonFunc.ResetAllControls(this);
            gbCompId.Visible = false;
            lblCompId.Text = string.Empty;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (View != null)
            {
                View.RowFilter = FilterQuery();
                dgvSubCategory.DataSource = View;
            }
        }

        private void dgvSubCategory_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblCompId.Text = dgvSubCategory.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtCompName.Text = dgvSubCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtSemi.Text = dgvSubCategory.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtRetail.Text = dgvSubCategory.Rows[e.RowIndex].Cells[3].Value.ToString();
                bool status = false;
                if (dgvSubCategory.Rows[e.RowIndex].Cells[4].Value.ToString() == "Yes")
                    status = true;
                rbActive.Checked = status;
                rbInactive.Checked = !status;
                gbCompId.Visible = true;
            }

        }
        #endregion

        private void txtSemi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtSemi);
        }

        private void txtRetail_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtRetail);
        }

        private void txtSemi_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtSemi.Text.Trim()))
            {
                if (Convert.ToDecimal(txtSemi.Text) < 0 || Convert.ToDecimal(txtSemi.Text) > 100)
                {
                    CommonFunc.DisplayErrorMessage("Invalid margin for Semi-Wholesale");
                    txtSemi.Focus();
                    return;
                }
            }
            if (!String.IsNullOrEmpty(txtRetail.Text.Trim()))
            {
                if (Convert.ToDecimal(txtRetail.Text) < 0 || Convert.ToDecimal(txtRetail.Text) > 100)
                {
                    CommonFunc.DisplayErrorMessage("Invalid margin for Retail");
                    txtRetail.Focus();
                    return;
                }
            }
        }
    }
}
