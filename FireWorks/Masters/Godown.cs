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
    public partial class Godown : Form
    {
        // private readonly ICommon dal = new CommonDal();
        DataView View = null;

        public Godown()
        {
            // dal = new CommonDal();
            InitializeComponent();
            this.Text = CommonFunc.ApplicationTitile;
            // show loading overlay while form initializes
            FireWorks.UI.UiHelpers.ShowLoading(this, "Loading items...");
            this.Text = CommonFunc.ApplicationTitile;
            try
            {
                // apply styling helpers at runtime for this form
                FireWorks.UI.UiHelpers.ApplyFormStyle(this);
            }
            catch { }
            //Common = CommonRepo;
            fillGrid();
            lblGodownId.Text = string.Empty;
            // hide loading overlay once initialization completes
            FireWorks.UI.UiHelpers.HideLoading(this);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtGodown.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Godown Name.");
                txtGodown.Focus();
                return;
            }
            if (!rbActive.Checked && !rbInactive.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Active / Inactive");
                return;
            }
            if (String.IsNullOrEmpty(txtAddress.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Godown Address.");
                txtAddress.Focus();
                return;
            }
            if (UserSessionEntity.UserID == null || UserSessionEntity.UserID <= 0)
            {
                CommonFunc.DisplayErrorMessage("Invalid Session");
                return;
            }

            CommonDal dal = new CommonDal();
            GodownEntity godown = new GodownEntity();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            godown.GodownName = txtGodown.Text.Trim();
            godown.GodownAddress = txtAddress.Text.Trim();
            godown.IsActive = rbActive.Checked;
            godown.Createdby = Convert.ToInt32(UserSessionEntity.UserID);
            if (!String.IsNullOrEmpty(lblGodownId.Text.Trim()))
            {
                godown.GodownId = Convert.ToInt32(lblGodownId.Text);
                godown.DMLType = 'U';
            }
            else
                godown.DMLType = 'I';
            dal.SaveGodown(godown, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.ResetAllControls(this);
                lblGodownId.Text = string.Empty;
                txtGodown.Focus();
                gbCatId.Visible = false;
                fillGrid();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
            }
            else
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
            //MessageBox.Show(alertMsgInfo.Message);
        }

        private void fillGrid()
        {
            CommonDal dal = new CommonDal();
            View = new DataView(dal.GetGodown(null));
            dgvCategory.DataSource = null;
            dgvCategory.DataSource = View;
            DisplayGrid(dgvCategory);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, false, false);

            objGrid.Columns["Id"].HeaderText = "Id";
            objGrid.Columns["Id"].Width = 50;
            objGrid.Columns["Id"].ReadOnly = true;

            objGrid.Columns["GodownDescription"].HeaderText = "Godown Name";
            objGrid.Columns["GodownDescription"].Width = 130;
            objGrid.Columns["GodownDescription"].ReadOnly = true;

            objGrid.Columns["GodownAddress"].HeaderText = "Address";
            objGrid.Columns["GodownAddress"].Width = 300;
            objGrid.Columns["GodownAddress"].ReadOnly = true;

            objGrid.Columns["IsActive"].HeaderText = "Is Active";
            objGrid.Columns["IsActive"].Width = 70;
            objGrid.Columns["IsActive"].ReadOnly = true;
        }

        private void dgvCategory_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblGodownId.Text = dgvCategory.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtGodown.Text = dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
                bool status = false;
                if (dgvCategory.Rows[e.RowIndex].Cells[2].Value.ToString() == "Yes")
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
            lblGodownId.Text = string.Empty;
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
                Filter.Append(" and [GodownDescription] LIKE '*" + txtSearch.Text + "*' ");
            }

            return Filter.ToString();
        }

        private void txtOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox TxtB = (TextBox)sender;
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
