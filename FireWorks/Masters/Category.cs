using FireWorks.UI;
using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using FireWorksEntity.Masters;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireWorks.Masters
{
    public partial class Category : Form
    {
        // private readonly ICommon dal = new CommonDal();
        DataView View = null;

        public Category()
        {
            // dal = new CommonDal();
            InitializeComponent();
            this.Text = CommonFunc.ApplicationTitile;
            // show loading overlay while form initializes
            FireWorks.UI.UiHelpers.ShowLoading(this, "Loading items...");

            fillGrid();
            lblCatId.Text = string.Empty;
            // hide loading overlay once initialization completes
            FireWorks.UI.UiHelpers.HideLoading(this);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtCategory.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Category Name.");
                txtCategory.Focus();
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
            CategoryEntity category = new CategoryEntity();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            category.CategoryName = txtCategory.Text.Trim();
            category.IsActive = rbActive.Checked;
            category.Createdby = Convert.ToInt32(UserSessionEntity.UserID);
            if (!String.IsNullOrEmpty(lblCatId.Text.Trim()))
            {
                category.CategoryId = Convert.ToInt32(lblCatId.Text);
                category.DMLType = 'U';
            }
            else
                category.DMLType = 'I';
            dal.SaveCategories(category, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.ResetAllControls(this);
                lblCatId.Text = string.Empty;
                txtCategory.Focus();
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
            View = new DataView(dal.GetCategories(null));
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

            objGrid.Columns["CatDescription"].HeaderText = "Category Name";
            objGrid.Columns["CatDescription"].Width = 250;
            objGrid.Columns["CatDescription"].ReadOnly = true;

            objGrid.Columns["IsActive"].HeaderText = "Is Active";
            objGrid.Columns["IsActive"].Width = 70;
            objGrid.Columns["IsActive"].ReadOnly = true;
        }

        private void dgvCategory_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblCatId.Text = dgvCategory.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtCategory.Text = dgvCategory.Rows[e.RowIndex].Cells[1].Value.ToString();
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
            lblCatId.Text = string.Empty;
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
                Filter.Append(" and [CatDescription] LIKE '*" + txtSearch.Text + "*' ");
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

        // call this from the form constructor after InitializeComponent()
        private void ApplyContrastingBackground()
        {
            // darker navy to slightly warmer navy
            var top = ColorTranslator.FromHtml("#071229");   // very dark navy
            var bottom = ColorTranslator.FromHtml("#0f2436"); // slightly lighter navy

            var bmp = new Bitmap(this.ClientSize.Width == 0 ? 1600 : this.ClientSize.Width,
                                 this.ClientSize.Height == 0 ? 900 : this.ClientSize.Height);
            using (var g = Graphics.FromImage(bmp))
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                       new Rectangle(Point.Empty, bmp.Size), top, bottom, System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, new Rectangle(Point.Empty, bmp.Size));
                // optional soft radial glow for center
                using (var glow = new System.Drawing.Drawing2D.PathGradientBrush(
                          new System.Drawing.Point[] {
                      new Point(bmp.Width/2 - 10, bmp.Height/4),
                      new Point(bmp.Width/2 + 10, bmp.Height/4) }))
                {
                    // PathGradientBrush requires a closed path; omitted for brevity.
                }
            }

            // assign background image (dispose old if any)
            this.BackgroundImage?.Dispose();
            this.BackgroundImage = bmp;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // ensure MdiClient background matches (if present)
            var mdiClient = this.Controls.OfType<MdiClient>().FirstOrDefault();
            if (mdiClient != null)
            {
                mdiClient.BackgroundImage = bmp;
                mdiClient.BackgroundImageLayout = ImageLayout.Stretch;
                mdiClient.BackColor = top;
            }
        }
        // Add inside the form class
        protected /*override*/ void OnPaintBackground231(PaintEventArgs e)
        {
            // do not call base to avoid default painting if you want full control
            var rect = this.ClientRectangle;
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                       rect,
                       Color.FromArgb(7, 18, 41),   // top (dark navy)
                       Color.FromArgb(15, 36, 54),  // bottom
                       System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // optional warm center vignette to mimic hero image: subtle radial gradient
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                int cx = rect.Width / 2;
                int cy = rect.Height / 4; // pull vignette upward
                int r = Math.Max(rect.Width, rect.Height) / 2;
                path.AddEllipse(cx - r, cy - r, r * 2, r * 2);
                using (var pgb = new System.Drawing.Drawing2D.PathGradientBrush(path))
                {
                    pgb.CenterColor = Color.FromArgb(28, 16, 0); // subtle warm center
                    pgb.SurroundColors = new[] { Color.Transparent };
                    pgb.FocusScales = new PointF(0.6f, 0.4f);
                    e.Graphics.FillEllipse(pgb, cx - r, cy - r, r * 2, r * 2);
                }
            }
        }
    }
}
