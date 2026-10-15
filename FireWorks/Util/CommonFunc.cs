using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FireWorks.Util
{
    public class CommonFunc
    {
        //public static string ApplicationTitile = (ShopDetails.ShopName ?? "Akshara solutions") + " - Version 1.0.0";
        public static string ApplicationTitile = "Sai Ganesh Traders - Version 1.0.1";

        #region Message Related
        public static void SaveMessage(string Message)
        {
            MessageBox.Show(Message, ApplicationTitile, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void DisplayErrorMessage(string Message)
        {
            MessageBox.Show(Message, ApplicationTitile, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void ResetAllControls(Control form)
        {
            try
            {
                foreach (Control ctrl in form.Controls)
                {
                    if (ctrl.Controls.Count > 0)
                        ResetAllControls(ctrl);
                    Reset(ctrl);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private static void Reset(Control ctrl)
        {
            if (ctrl is TextBox tb)
            {
                if (tb != null)
                    tb.ResetText();
            }
            else if (ctrl is ComboBox cb)
            {
                if (cb != null)
                    cb.SelectedIndex = 0;
            }
            else if (ctrl is PictureBox pb)
            {
                if (pb.Image != null)
                {
                    pb.Image = null;
                }

            }
            else if (ctrl is DateTimePicker dt)
            {
                dt.Text = "";
            }
            else if (ctrl is ListView lstview)
            {
                if (lstview.Items.Count > 0)
                    lstview.Items.Clear();
            }
            else if (ctrl is CheckBox chk)
            {
                if (chk.Checked == true)
                    chk.Checked = false;
            }
            else if (ctrl is RadioButton rb)
            {
                if (rb.Checked == true)
                    rb.Checked = false;
            }
        }

        public static void FormatGridLook(DataGridView grdObj, Boolean AddRows,
            Boolean DeleteRows, Boolean ColumnsClear)
        {
            if (ColumnsClear)
            {
                grdObj.Rows.Clear();
                grdObj.Columns.Clear();
                grdObj.AutoGenerateColumns = false;
            }

            grdObj.AllowUserToResizeRows = false;
            grdObj.ShowCellToolTips = true;
            grdObj.RowHeadersVisible = false;
            grdObj.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdObj.MultiSelect = false;

            grdObj.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grdObj.AllowUserToAddRows = AddRows;
            grdObj.AllowUserToDeleteRows = DeleteRows;

            grdObj.ColumnHeadersHeight = 25;
            grdObj.BorderStyle = BorderStyle.FixedSingle;
        }
        #endregion

        public static void CategoryFill(ComboBox cmbbox)
        {
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetCategories(true);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow drow = dt.NewRow();
                    drow["CatDescription"] = "--Select Category--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Category(s) Not found");
                    DataRow drow = dt.NewRow();
                    drow["CatDescription"] = "--Select Category--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                cmbbox.DisplayMember = "CatDescription";
                cmbbox.ValueMember = "Id";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public static void SubCategoryFill(ComboBox cmbbox, int? categoryId, bool isEmpty = false)
        {
            if (isEmpty || categoryId == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[2] {
                    new DataColumn("Id", typeof(int)),
                    new DataColumn("SubCatDescription", typeof(string))
                });
                DataRow drow = dt.NewRow();
                drow["SubCatDescription"] = "--Select Sub-Category--";
                //drow["Id"] = null;
                dt.Rows.InsertAt(drow, 0);
                drow = null;

                cmbbox.DataSource = null;
                cmbbox.DataSource = dt;

                cmbbox.DisplayMember = "SubCatDescription";
                cmbbox.ValueMember = "Id";
                cmbbox.SelectedIndex = 0;
                return;
            }
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetSubCategories(true, categoryId);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow drow = dt.NewRow();
                    drow["SubCatDescription"] = "--Select Sub-Category--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Sub-Category(s) Not found");
                    DataRow drow = dt.NewRow();
                    drow["SubCatDescription"] = "--Select Sub-Category--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                cmbbox.DisplayMember = "SubCatDescription";
                cmbbox.ValueMember = "Id";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public static void CompaniesFill(ComboBox cmbbox)
        {
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetCompanies(true);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow drow = dt.NewRow();
                    drow["CompanyName"] = "--Select Company--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Company(s) Not found");
                    DataRow drow = dt.NewRow();
                    drow["CompanyName"] = "--Select Company--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                cmbbox.DisplayMember = "CompanyName";
                cmbbox.ValueMember = "Id";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public static void ItemTypeFill(ComboBox cmbbox)
        {
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetItemType(true);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow drow = dt.NewRow();
                    drow["TypeName"] = "--Select Item Type--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Item(s) Not found");
                    DataRow drow = dt.NewRow();
                    drow["TypeName"] = "--Select Item Type--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                cmbbox.DisplayMember = "TypeName";
                cmbbox.ValueMember = "Id";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public static void HSNFill(ComboBox cmbbox)
        {
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetHSN(true);
                cmbbox.DataSource = null;
                DataRow drow = dt.NewRow();
                drow["HSNCode"] = "--Select HSN Code--";
                dt.Rows.InsertAt(drow, 0);
                drow = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Item(s) Not found");
                }
                cmbbox.DisplayMember = "HSNCode";
                cmbbox.ValueMember = "HSNId";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public static void ItemTypeByItem(ComboBox cmbbox, int ItemId)
        {
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetItemTypeByItem(ItemId);
                DataRow drow = dt.NewRow();
                drow["TypeName"] = "--Select Item Type--";
                //drow["Id"] = null;
                dt.Rows.InsertAt(drow, 0);
                drow = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Item Type Not found");

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                cmbbox.DisplayMember = "TypeName";
                cmbbox.ValueMember = "Id";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public static string GetItemTypeDesc(int ItemId, int Quantity)
        {
            CommonDal dal = new CommonDal();
            return dal.GetItemTypeDesc(ItemId, Quantity);
        }

        public static void RoleFill(ComboBox cmbbox)
        {
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetRole();
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow drow = dt.NewRow();
                    drow["RoleName"] = "--Select Role--";
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Role(s) Not found");
                    DataRow drow = dt.NewRow();
                    drow["RoleName"] = "--Select Role--";
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                cmbbox.DisplayMember = "RoleName";
                cmbbox.ValueMember = "RoleID";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        public static bool isNumber(char ch, string text)
        {
            bool res = true;
            //char decimalChar = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);

            //check if it's a decimal separator and if doesn't already have one in the text string
            //if (ch == decimalChar && text.IndexOf(decimalChar) != -1)
            //{
            //    res = false;
            //    return res;
            //}

            //check if it´s a digit, decimal separator and backspace
            if (!Char.IsDigit(ch) && ch != (char)Keys.Back && ch != 45)
                res = false;

            return res;
        }

        public static bool IsDecimal(KeyPressEventArgs e, TextBox textBox)
        {
            bool result = false;
            //Regex reg = new Regex(@"^-?\d+[.]?\d*$");
            Regex reg = new Regex(@"^-?\d+[.]?\d*$");
            if (char.IsControl(e.KeyChar))
                return result;
            if (!reg.IsMatch(textBox.Text.Insert(textBox.SelectionStart, e.KeyChar.ToString()) + "1"))
                result = true;
            return result;
        }

        public static void fillCustomer(ComboBox cmbbox)
        {
            //CommonFunc.ItemTypeFill(cbItemType);
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[2] {
                    new DataColumn("Id", typeof(int)),
                    new DataColumn("CustomerType", typeof(string))
                });
            dataTable.Rows.Add(null, "--Select--");
            dataTable.Rows.Add(1, "Alpha");
            dataTable.Rows.Add(2, "Beta");
            dataTable.Rows.Add(3, "Gamma");
            cmbbox.DataSource = dataTable;
            cmbbox.DisplayMember = "CustomerType";
            cmbbox.ValueMember = "Id";
        }
        public static void FillGodwon(ComboBox cmbbox)
        {
            try
            {
                CommonDal dal = new CommonDal();
                DataTable dt = dal.GetGodown(true);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow drow = dt.NewRow();
                    drow["GodownDescription"] = "--Select Godown--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                else
                {
                    DisplayErrorMessage("Godown(s) Not found");
                    DataRow drow = dt.NewRow();
                    drow["GodownDescription"] = "--Select Godown--";
                    //drow["Id"] = null;
                    dt.Rows.InsertAt(drow, 0);
                    drow = null;

                    cmbbox.DataSource = null;
                    cmbbox.DataSource = dt;
                }
                cmbbox.DisplayMember = "GodownDescription";
                cmbbox.ValueMember = "Id";
                cmbbox.SelectedIndex = 0;
            }
            catch
            {

            }
        }
    }
}
