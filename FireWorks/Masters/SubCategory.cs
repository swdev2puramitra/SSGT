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
    public partial class SubCategory : Form
    {
        DataView View = null;
        public SubCategory()
        {
            InitializeComponent();
            CommonFunc.CategoryFill(cbCategory);
            this.Text = CommonFunc.ApplicationTitile;
            fillGrid();
        }

        #region Methods
        private void fillGrid()
        {
            CommonDal dal = new CommonDal();
            View = new DataView(dal.GetSubCategories(null, null));
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

            objGrid.Columns["SubCatDescription"].HeaderText = "Sub Category";
            objGrid.Columns["SubCatDescription"].Width = 250;
            objGrid.Columns["SubCatDescription"].ReadOnly = true;

            objGrid.Columns["CategoryId"].HeaderText = "CategoryId";
            objGrid.Columns["CategoryId"].Width = 60;
            objGrid.Columns["CategoryId"].ReadOnly = true;
            objGrid.Columns["CategoryId"].Visible = false;

            objGrid.Columns["CategoryName"].HeaderText = "Category Name";
            objGrid.Columns["CategoryName"].Width = 200;
            objGrid.Columns["CategoryName"].ReadOnly = true;

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
                Filter.Append(" and [SubCatDescription] LIKE '*" + txtSearch.Text + "*' ");
            }

            return Filter.ToString();
        }
        #endregion

        #region Events
        private void btnSave_Click(object sender, EventArgs e)
        {
            int catId = 0;
            if (cbCategory.SelectedValue == null || !int.TryParse(cbCategory.SelectedValue.ToString(), out catId) || catId <= 0)
            {
                CommonFunc.DisplayErrorMessage("Select Category");
                return;
            }

            if (String.IsNullOrEmpty(txtSubCategory.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Sub-Category Name.");
                return;
            }

            if (!rbActive.Checked && !rbInactive.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Active / Inactive");
                return;
            }
            CommonDal dal = new CommonDal();
            SubCategoryEntity category = new SubCategoryEntity();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            category.SubCategoryName = txtSubCategory.Text.Trim();
            category.IsActive = rbActive.Checked;
            category.CategoryId = catId;
            if (!String.IsNullOrEmpty(lblSubCatId.Text.Trim()))
            {
                category.SubCategoryId = Convert.ToInt32(lblSubCatId.Text);
                category.DMLType = 'U';
            }
            else
                category.DMLType = 'I';
            dal.SaveSubCategories(category, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.ResetAllControls(this);
                lblSubCatId.Text = string.Empty;
                txtSubCategory.Focus();
                gbCatId.Visible = false;
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
            lblSubCatId.Text = string.Empty;
        }

        private void dgvSubCategory_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblSubCatId.Text = dgvSubCategory.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtSubCategory.Text = dgvSubCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
                cbCategory.SelectedValue= dgvSubCategory.Rows[e.RowIndex].Cells[2].Value.ToString();
                bool status = false;
                if (dgvSubCategory.Rows[e.RowIndex].Cells[4].Value.ToString() == "Yes")
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
