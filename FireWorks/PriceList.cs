using FireWorks.Util;
using FireWorksDal.DataLayer;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class PriceList : Form
    {
        int CstType = 0;
        DataView View = null;
        public PriceList(int CustomerType)
        {
            InitializeComponent();
            CstType = CustomerType;

            switch (CustomerType)
            {
                case 1:
                    this.Text = "Alpha";
                    break;
                case 2:
                    this.Text = "Beta";
                    break;
                case 3:
                    this.Text = "Gamma";
                    break;
            }
            bindCombo();
            FillGrid();
            DisplayGrid(dgvInventory);
        }

        private void bindCombo()
        {
            CommonFunc.CategoryFill(cbCategory);
            CommonFunc.SubCategoryFill(cbSubCategory, null, true);
            CommonFunc.CompaniesFill(cbCompany);

            txtItemName.Focus();
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            //objGrid.ReadOnly = true;
            CommonFunc.FormatGridLook(objGrid, false, false, false);

            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 70;
            objGrid.Columns["SlNo"].ReadOnly = true;
            objGrid.Columns["SlNo"].DisplayIndex = 0;

            objGrid.Columns["ItemCode"].HeaderText = "Item Code";
            objGrid.Columns["ItemCode"].Width = 90;
            objGrid.Columns["ItemCode"].ReadOnly = true;
            objGrid.Columns["ItemCode"].DisplayIndex = 1;

            objGrid.Columns["ItemName"].HeaderText = "Item Name";
            objGrid.Columns["ItemName"].Width = 240;
            objGrid.Columns["ItemName"].ReadOnly = true;
            objGrid.Columns["ItemName"].DisplayIndex = 2;

            objGrid.Columns["ItemPrice"].HeaderText = "Item Price";
            objGrid.Columns["ItemPrice"].Width = 100;
            objGrid.Columns["ItemPrice"].ReadOnly = true;
            objGrid.Columns["ItemPrice"].DisplayIndex = 3;

            objGrid.Columns["TypeName"].HeaderText = "Item Type";
            objGrid.Columns["TypeName"].Width = 100;
            objGrid.Columns["TypeName"].ReadOnly = true;
            objGrid.Columns["TypeName"].DisplayIndex = 4;

            objGrid.Columns["CompanyName"].HeaderText = "Company Name";
            objGrid.Columns["CompanyName"].Width = 150;
            objGrid.Columns["CompanyName"].ReadOnly = true;
            objGrid.Columns["CompanyName"].DisplayIndex = 5;

            objGrid.Columns["CatDescription"].HeaderText = "Category";
            objGrid.Columns["CatDescription"].Width = 120;
            objGrid.Columns["CatDescription"].ReadOnly = true;
            objGrid.Columns["CatDescription"].DisplayIndex = 6;

            objGrid.Columns["SubCatDescription"].HeaderText = "Sub Category";
            objGrid.Columns["SubCatDescription"].Width = 150;
            objGrid.Columns["SubCatDescription"].ReadOnly = true;
            objGrid.Columns["SubCatDescription"].DisplayIndex = 7;

            objGrid.Columns["ConversionName"].HeaderText = "Conversion Name";
            objGrid.Columns["ConversionName"].Width = 120;
            objGrid.Columns["ConversionName"].ReadOnly = true;
            objGrid.Columns["ConversionName"].DisplayIndex = 8;

            objGrid.Columns["CompanyId"].HeaderText = "CompanyId";
            objGrid.Columns["CompanyId"].Width = 60;
            objGrid.Columns["CompanyId"].ReadOnly = true;
            objGrid.Columns["CompanyId"].DisplayIndex = 9;

            objGrid.Columns["CategoryId"].HeaderText = "CategoryId";
            objGrid.Columns["CategoryId"].Width = 60;
            objGrid.Columns["CategoryId"].ReadOnly = true;
            objGrid.Columns["CategoryId"].DisplayIndex = 10;

            objGrid.Columns["SubCategoryID"].HeaderText = "SubCategoryID";
            objGrid.Columns["SubCategoryID"].Width = 60;
            objGrid.Columns["SubCategoryID"].ReadOnly = true;
            objGrid.Columns["SubCategoryID"].DisplayIndex = 11;

            objGrid.Columns["ItemType"].HeaderText = "ItemType";
            objGrid.Columns["ItemType"].Width = 60;
            objGrid.Columns["ItemType"].ReadOnly = true;
            objGrid.Columns["ItemType"].DisplayIndex = 12;

            string[] hideCols = new string[] { "CompanyId", "CategoryId", "SubCategoryID", "ItemType" };
            foreach (string hide in hideCols)
            {
                if (objGrid.Columns[hide] != null)
                    objGrid.Columns[hide].Visible = false;
            }
            //int width = 0;
            //foreach (DataGridViewColumn column in objGrid.Columns)
            //{
            //    if (column.Visible)
            //        width += column.Width;
            //}
            //objGrid.Width = width;
        }

        private string FilterQuery()
        {
            StringBuilder Filter = new StringBuilder();
            Filter.Append("1=1");
            if (txtItemName.Text.Trim().Length > 0)
            {
                Filter.Append(" and ([ItemName] LIKE '*" + txtItemName.Text + "*' or [ItemCode] LIKE '*" + txtItemName.Text + "*')");
            }
            if (cbCategory.SelectedIndex > 0)
                Filter.Append(" and [CategoryId] = " + cbCategory.SelectedValue);

            if (cbSubCategory.SelectedIndex > 0)
                Filter.Append(" and [SubCategoryID] = " + cbSubCategory.SelectedValue);

            if (cbCompany.SelectedIndex > 0)
                Filter.Append(" and [CompanyId] = " + cbCompany.SelectedValue);

            return Filter.ToString();
        }

        private void FillGrid()
        {
            CommonDal common = new CommonDal();
            View = new DataView(common.GetPriceList(CstType));
            updateDGVData();
        }
        private void txtItemName_TextChanged(object sender, EventArgs e)
        {
            updateDGVData();
        }

        private void cbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateDGVData();
        }

        private void updateDGVData()
        {
            if (View != null)
            {
                View.RowFilter = FilterQuery();
                int i = 1;

                foreach (DataGridViewRow row in dgvInventory.Rows)
                {
                    row.Cells["SlNo"].Value = (i++).ToString();
                }

                dgvInventory.DataSource = View;
            }
            DisplayGrid(dgvInventory);
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonFunc.SubCategoryFill(cbSubCategory, cbCategory.SelectedValue as int?, false);
            updateDGVData();
        }
    }
}
