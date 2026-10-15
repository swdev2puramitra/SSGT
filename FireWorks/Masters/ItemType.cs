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
    public partial class ItemType : Form
    {
        DataView View = null;
        public ItemType()
        {
            InitializeComponent();
            this.Text = CommonFunc.ApplicationTitile;
            fillGrid();
        }

        #region Methods
        private void fillGrid()
        {
            CommonDal dal = new CommonDal();
            View = new DataView(dal.GetItemType(null));
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

            objGrid.Columns["TypeName"].HeaderText = "Item Type";
            objGrid.Columns["TypeName"].Width = 250;
            objGrid.Columns["TypeName"].ReadOnly = true;

            objGrid.Columns["IsActive"].HeaderText = "Is Active";
            objGrid.Columns["IsActive"].Width = 70;
            objGrid.Columns["IsActive"].ReadOnly = true;
        }

        private string FilterQuery()
        {
            StringBuilder Filter = new StringBuilder();
            Filter.Append("1=1");
            if (txtSearch.Text.Trim().Length > 0)
            {
                Filter.Append(" and [TypeName] LIKE '*" + txtSearch.Text + "*' ");
            }

            return Filter.ToString();
        }
        #endregion

        #region Events
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtTypeName.Text.Trim()))
            {
                MessageBox.Show("Enter Item Type.");
                return;
            }

            if (!rbActive.Checked && !rbInactive.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Active / Inactive");
                return;
            }
            CommonDal dal = new CommonDal();
            ItemTypeEntity category = new ItemTypeEntity();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            category.ItemTypeName = txtTypeName.Text.Trim();
            category.IsActive = rbActive.Checked;
            if (!String.IsNullOrEmpty(lblTypeId.Text.Trim()))
            {
                category.ItemTypeId = Convert.ToInt32(lblTypeId.Text);
                category.DMLType = 'U';
            }
            else
                category.DMLType = 'I';
            dal.SaveItemType(category, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.ResetAllControls(this);
                gbCatId.Visible = false;
                lblTypeId.Text = String.Empty;
                fillGrid();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
            }
            else
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CommonFunc.ResetAllControls(this);
            gbCatId.Visible = false;
            lblTypeId.Text = String.Empty;
        }

        private void dgvSubCategory_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblTypeId.Text = dgvSubCategory.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtTypeName.Text = dgvSubCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
                bool status = false;
                if (dgvSubCategory.Rows[e.RowIndex].Cells[2].Value.ToString() == "Yes")
                    status = true;
                rbActive.Checked = status;
                rbInactive.Checked = !status;
                gbCatId.Visible = true;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (View != null)
            {
                View.RowFilter = FilterQuery();
                dgvSubCategory.DataSource = View;
            }
        } 
        #endregion
    }
}
