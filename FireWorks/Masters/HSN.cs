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
    public partial class HSN : Form
    {
        // private readonly ICommon dal = new CommonDal();
        DataView View = null;

        public HSN()
        {
            // dal = new CommonDal();
            InitializeComponent();
            this.Text = CommonFunc.ApplicationTitile;
            //Common = CommonRepo;
            fillGrid();
            lblHSNId.Text = string.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtHSNCode.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter HSN Name.");
                txtHSNCode.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtCGST.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter CGST.");
                txtCGST.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtSGST.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter SGST.");
                txtSGST.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtIGST.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter IGST.");
                txtIGST.Focus();
                return;
            }
            if (!rbActive.Checked && !rbInactive.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Active / Inactive");
                return;
            }
            if (UserSessionEntity.UserID == null || UserSessionEntity.UserID <= 0)
            {
                CommonFunc.DisplayErrorMessage("Invalid Session");
                return;
            }

            CommonDal dal = new CommonDal();
            HSNEntity entity = new HSNEntity();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            entity.HSNCode = txtHSNCode.Text.Trim();
            entity.CGST = Convert.ToDecimal(txtCGST.Text.Trim());
            entity.SGST = Convert.ToDecimal(txtSGST.Text.Trim());
            entity.IGST = Convert.ToDecimal(txtIGST.Text.Trim());
            entity.IsActive = rbActive.Checked;
            entity.Createdby = Convert.ToInt32(UserSessionEntity.UserID);
            if (!String.IsNullOrEmpty(lblHSNId.Text.Trim()))
            {
                entity.HSNId = Convert.ToInt32(lblHSNId.Text);
                entity.DMLType = 'U';
            }
            else
                entity.DMLType = 'I';
            dal.SaveHSN(entity, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.ResetAllControls(this);
                txtHSNCode.Focus();
                lblHSNId.Text = string.Empty;
                gbCatId.Visible = false;
                fillGrid();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
            }
            else
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);            
        }

        private void fillGrid()
        {
            CommonDal dal = new CommonDal();
            View = new DataView(dal.GetHSN(null));
            dgvCategory.DataSource = null;
            dgvCategory.DataSource = View;
            DisplayGrid(dgvCategory);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, false, false);

            objGrid.Columns["HSNId"].HeaderText = "HSN Id";
            objGrid.Columns["HSNId"].Width = 70;
            objGrid.Columns["HSNId"].ReadOnly = true;
            objGrid.Columns["HSNId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["HSNCode"].HeaderText = "HSN Name";
            objGrid.Columns["HSNCode"].Width = 150;
            objGrid.Columns["HSNCode"].ReadOnly = true;

            objGrid.Columns["CGST"].HeaderText = "CGST";
            objGrid.Columns["CGST"].Width = 70;
            objGrid.Columns["CGST"].ReadOnly = true;
            objGrid.Columns["CGST"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["SGST"].HeaderText = "SGST";
            objGrid.Columns["SGST"].Width = 70;
            objGrid.Columns["SGST"].ReadOnly = true;
            objGrid.Columns["SGST"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["IGST"].HeaderText = "IGST";
            objGrid.Columns["IGST"].Width = 70;
            objGrid.Columns["IGST"].ReadOnly = true;
            objGrid.Columns["IGST"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["IsActive"].HeaderText = "Is Active";
            objGrid.Columns["IsActive"].Width = 70;
            objGrid.Columns["IsActive"].ReadOnly = true;
        }

        private void dgvCategory_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblHSNId.Text = dgvCategory.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtHSNCode.Text = dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtCGST.Text = dgvCategory.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtSGST.Text = dgvCategory.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtIGST.Text = dgvCategory.Rows[e.RowIndex].Cells[4].Value.ToString();
                bool status = false;
                if (dgvCategory.Rows[e.RowIndex].Cells[5].Value.ToString() == "Yes")
                    status = true;
                rbActive.Checked = status;
                rbInactive.Checked = !status;
                gbCatId.Visible = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CommonFunc.ResetAllControls(this);
            gbCatId.Visible = false;
            lblHSNId.Text = string.Empty;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (View != null)
            {
                View.RowFilter = FilterQuery();
                dgvCategory.DataSource = View;
            }
        }

        private string FilterQuery()
        {
            StringBuilder Filter = new StringBuilder();
            Filter.Append("1=1");
            if (txtSearch.Text.Trim().Length > 0)
            {
                Filter.Append(" and [Name] LIKE '*" + txtSearch.Text + "*' ");
            }

            return Filter.ToString();
        }


        private void txtCGST_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtCGST);
        }

        private void txtSGST_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtSGST);
        }

        private void txtIGST_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtIGST);
        }
    }
}
