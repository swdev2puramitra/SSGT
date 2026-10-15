using CrystalDecisions.Shared;
using FireWorks.Reports;
using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class Billing : Form
    {
        int customerType = 0;
        int editIndex = -1;//rows = 1,
        private void Initialization()
        {
            //MdiParent = new MDIFireWorks();
            InitializeComponent();
            // show loading overlay during initial UI setup
            FireWorks.UI.UiHelpers.ShowLoading(this, "Loading billing form...");

            // Apply runtime UI styling for this form only (non-destructive)
            try
            {
                // Apply form-wide styling automatically
                FireWorks.UI.UiHelpers.ApplyFormStyle(this);
            }
            catch { }

            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int w = Width >= screen.Width ? screen.Width : (screen.Width + Width) / 2;
            int h = Height >= screen.Height ? screen.Height : (screen.Height + Height) / 2;
            this.Location = new Point((screen.Width - w) / 2, (screen.Height - h) / 2);
            this.Size = new Size(w, h);
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterScreen;
            txtItemCode.Enabled = false;
            cbCustomer.Focus();
            CommonFunc.fillCustomer(cbCustomer);
            DisplayGrid(dgvItemsList);

            if (UserSessionEntity.RoleId != 1)
            {
                rbInvoice.Enabled = false;
                rbInvoice.Checked = false;
            }

            // enable incremental filtering on the items grid as user types in item code
            //try { txtItemCode.TextChanged += txtItemCode_TextChanged; } catch { }

            // hide loading overlay after initial display
            FireWorks.UI.UiHelpers.HideLoading(this);
        }

        public Billing()
        {
            Initialization();
            StopSorting();
            rbEstimate.Checked = true;
        }

        public Billing(Int64 BillId, bool isCopy = false)
        {
            Initialization();
            BillDal billDal = new BillDal();
            Bill bill = billDal.GetBillDetailsById(BillId);

            if (bill != null)
            {
                if (!isCopy)
                {
                    txtBillId.Text = bill.BillId.ToString();
                    txtPhone.Text = bill.PhoneNo == 0 ? "" : bill.PhoneNo.ToString();
                    cbCustomer.SelectedValue = bill.CustomerType.ToString();
                    cbCustomer.Enabled = false;
                    txtCustomer.Text = bill.Name;
                    txtAddress.Text = bill.Address;
                    rbEstimate.Checked = Convert.ToChar(bill.IsEstimateInvoice) == 'E';
                    rbInvoice.Checked = !rbEstimate.Checked;
                    txtPhone2.Text = bill.Mobile2.ToString();
                    dgvItemsList.Rows.Clear();
                }
                foreach (BilledItems billed in bill.billedItems)
                {
                    /*
                     * SlNo,ItemCode,ItemName,ItemType,Detailed,Quantity,IndividualCost,TotalCost,ItemId,
                     * ItemTypeId,PrevQty,ItemPrice,delete,CGST,SGST,IGST,ChangedPrice,SellType,SellQty,Edit,
                     * IsBilled,BilledPrice
                     */

                    dgvItemsList.Rows.Add(new string[] {
                        billed.SlNo.ToString(),billed.ItemCode,billed.ItemName,billed.ItemTypeName,billed.Detailed,
                        billed.Quantity.ToString(),(billed.ChangedPrice==0? billed.IndividualCost:billed.ChangedPrice).ToString(),
                        billed.TotalCost.ToString(),billed.ItemId.ToString(),billed.ItemTypeId.ToString(),billed.Quantity.ToString(),
                        billed.ItemCost.ToString(),null,billed.CGST.ToString(),billed.SGST.ToString(),billed.IGST.ToString(),
                        billed.ChangedPrice.ToString(),billed.SellTypeId.ToString(),billed.SellQty.ToString(),null,"Y",
                        //(billed.ChangedPrice==0? billed.IndividualCost:billed.ChangedPrice).ToString()
                        billed.OriginalCost.ToString()
                        });
                }

                DisplayGrid(dgvItemsList);
                lblBillNo.Text = bill.BillNo;
                if (rbEstimate.Checked)
                {
                    gbInvoice.Visible = false;
                }
                else
                {
                    rbEstimate.Enabled = false;
                    gbInvoice.Enabled = true;
                    gbInvoice.Visible = true;
                    if (UserSessionEntity.RoleId == 1)
                    {
                        btnRemove.Visible = true;
                        btnCancel.Visible = true;
                    }
                    if (bill.DiscountPercentage > 0)
                        lblTaxableAmt.Text = Math.Round(Convert.ToDecimal(bill.BillAmount) - (Convert.ToDecimal(bill.DiscountPercentage) * Convert.ToDecimal(bill.BillAmount) / 100), 2).ToString();
                    else
                        lblTaxableAmt.Text = "0.00";
                    rbState.Checked = Convert.ToBoolean(bill.IsInState);
                    rbOut.Checked = !rbState.Checked;

                    txtGSTIN.Text = bill.CustGSTIN;
                    txtInvoice.Text = bill.InvoiceNo;
                    txtPincode.Text = bill.Pincode;
                    txtPOS.Text = bill.POS;
                    txtStCd.Text = bill.StateCode;
                    txtLocation.Text = bill.Location;
                }
                txtBillAmt.Text = Math.Round(bill.BillAmount, 2).ToString();
                txtDiscPerItem.Text = Math.Round(Convert.ToDecimal(bill.DiscountPerItem), 2).ToString();
                txtDiscPercentage.Text = Math.Round(Convert.ToDecimal(bill.DiscountPercentage), 2).ToString();

                //txtFinalBill.Text = Math.Round(bill.TotalBillAmount, 2).ToString();

                fillCombo();
            }
            else
            {
                CommonFunc.DisplayErrorMessage("Unable to fetch Bill details");
            }

            StopSorting();
        }

        public void fillCombo()
        {
            //CommonFunc.ItemTypeFill(cbItemType);
            DataTable dataTable = new DataTable();
            dataTable.Columns.AddRange(new DataColumn[2] {
                    new DataColumn("ItemTypeId", typeof(int)),
                    new DataColumn("TypeName", typeof(string))
                });
            dataTable.Rows.Add(null, "--Select Item Type--");
            cbItemType.DataSource = dataTable;
            cbItemType.DisplayMember = "TypeName";
            cbItemType.ValueMember = "ItemTypeId";
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            //objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, true, false);
            objGrid.ColumnHeadersHeight = 40;

            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 40;
            objGrid.Columns["SlNo"].ReadOnly = true;

            objGrid.Columns["ItemCode"].HeaderText = "Item Code";
            objGrid.Columns["ItemCode"].Width = 80;
            objGrid.Columns["ItemCode"].ReadOnly = true;

            objGrid.Columns["ItemName"].HeaderText = "Item Name";
            objGrid.Columns["ItemName"].Width = 220;
            objGrid.Columns["ItemName"].ReadOnly = true;

            objGrid.Columns["ItemType"].HeaderText = "Item Type";
            objGrid.Columns["ItemType"].Width = 70;
            objGrid.Columns["ItemType"].ReadOnly = true;

            objGrid.Columns["Detailed"].HeaderText = "Detailed";
            objGrid.Columns["Detailed"].Width = 200;
            objGrid.Columns["Detailed"].ReadOnly = true;

            objGrid.Columns["Quantity"].HeaderText = "Quantity";
            objGrid.Columns["Quantity"].Width = 60;
            objGrid.Columns["Quantity"].ReadOnly = false;
            objGrid.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["IndividualCost"].HeaderText = "Individual Cost";
            objGrid.Columns["IndividualCost"].Width = 75;
            objGrid.Columns["IndividualCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["TotalCost"].HeaderText = "Total Cost";
            objGrid.Columns["TotalCost"].Width = 80;
            objGrid.Columns["TotalCost"].ReadOnly = true;
            objGrid.Columns["TotalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            objGrid.Columns["ItemId"].HeaderText = "Item Id";
            objGrid.Columns["ItemId"].Width = 70;
            objGrid.Columns["ItemId"].ReadOnly = true;
            objGrid.Columns["ItemId"].Visible = false;

            objGrid.Columns["ItemTypeId"].HeaderText = "Item Type";
            objGrid.Columns["ItemTypeId"].Width = 20;
            objGrid.Columns["ItemTypeId"].ReadOnly = true;
            objGrid.Columns["ItemTypeId"].Visible = false;

            objGrid.Columns["PrevQty"].HeaderText = "Prev Qty";
            objGrid.Columns["PrevQty"].Width = 70;
            objGrid.Columns["PrevQty"].ReadOnly = true;
            objGrid.Columns["PrevQty"].Visible = false;

            objGrid.Columns["ItemPrice"].HeaderText = "Item Price";
            objGrid.Columns["ItemPrice"].Width = 70;
            objGrid.Columns["ItemPrice"].ReadOnly = true;
            objGrid.Columns["ItemPrice"].Visible = false;

            objGrid.Columns["Delete"].HeaderText = "Delete";
            objGrid.Columns["Delete"].Width = 60;
            objGrid.Columns["Delete"].ReadOnly = true;

            objGrid.Columns["CGST"].HeaderText = "CGST";
            objGrid.Columns["CGST"].ReadOnly = true;
            objGrid.Columns["CGST"].Visible = false;

            objGrid.Columns["SGST"].HeaderText = "SGST";
            objGrid.Columns["SGST"].ReadOnly = true;
            objGrid.Columns["SGST"].Visible = false;

            objGrid.Columns["IGST"].HeaderText = "IGST";
            objGrid.Columns["IGST"].ReadOnly = true;
            objGrid.Columns["IGST"].Visible = false;

            objGrid.Columns["ChangedPrice"].HeaderText = "Changed Price";
            objGrid.Columns["ChangedPrice"].ReadOnly = true;
            objGrid.Columns["ChangedPrice"].Visible = false;

            objGrid.Columns["SellType"].HeaderText = "Sell Type";
            objGrid.Columns["SellType"].ReadOnly = true;
            objGrid.Columns["SellType"].Visible = false;

            objGrid.Columns["SellQty"].HeaderText = "ChangedPrice";
            objGrid.Columns["SellQty"].ReadOnly = true;
            objGrid.Columns["SellQty"].Visible = false;

            objGrid.Columns["Edit"].HeaderText = "Edit";
            objGrid.Columns["Edit"].ReadOnly = true;
            objGrid.Columns["Edit"].Width = 60;

            objGrid.Columns["IsBilled"].HeaderText = "Is Billed";
            objGrid.Columns["IsBilled"].ReadOnly = true;
            objGrid.Columns["IsBilled"].Visible = false;

            objGrid.Columns["BilledPrice"].HeaderText = "BilledPrice";
            objGrid.Columns["BilledPrice"].ReadOnly = true;
            objGrid.Columns["BilledPrice"].Visible = false;

            objGrid.Columns["StockPosition"].HeaderText = "StockPosition";
            objGrid.Columns["StockPosition"].ReadOnly = true;
            objGrid.Columns["StockPosition"].Visible = false;
        }

        private void txtItemCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtItemId.Text = String.Empty;
                using (ItemSearch frm = new ItemSearch(txtItemCode.Text, customerType, true))
                {
                    if (!frm.Result)
                        frm.ShowDialog();
                    if (frm.Result)
                    {
                        ItemsEntity response = frm.items;
                        CommonFunc.ItemTypeByItem(cbItemType, Convert.ToInt32(response.ItemId));
                        if (cbItemType.Items.Count > 1)
                        {
                            if (response.CGST <= 0 || response.SGST == 0 || response.IGST == 0)
                            {
                                CommonFunc.DisplayErrorMessage("HSN code is not mapped");
                                return;
                            }
                            txtItemCode.Text = response.ItemCode;
                            txtItemName.Text = response.ItemName;
                            cbItemType.SelectedValue = response.ItemType;
                            cbItemType.Enabled = true;
                            txtItemPrice.Text = response.ItemPrice.ToString();
                            txtItemId.Text = response.ItemId.ToString();
                            cbCustomer.Enabled = false;
                            lblItemCGST.Text = response.CGST.ToString();
                            lblItemSGST.Text = response.SGST.ToString();
                            lblItemIGST.Text = response.IGST.ToString();
                            lblStock.Text = response.Stock.ToString();
                            txtQuantity.Focus();
                        }
                        else
                        {
                            CommonFunc.DisplayErrorMessage("Item Type is not identified");
                        }
                    }
                }
            }
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (!String.IsNullOrEmpty(txtQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtItemPrice.Text.Trim()))
                {
                    if (cbItemType.SelectedValue == null || String.IsNullOrEmpty(cbItemType.SelectedValue.ToString()))
                    {
                        CommonFunc.DisplayErrorMessage("Select Item Type");
                        return;
                    }
                    txtTotal.Text = (Convert.ToDecimal(txtItemPrice.Text) * Convert.ToDecimal(txtQuantity.Text)).ToString();
                    int selectedRowId = -1;
                    foreach (DataGridViewRow viewRow in dgvItemsList.Rows)
                    {
                        if (viewRow.Cells["ItemId"].Value.ToString() == txtItemId.Text)
                        {
                            //selectedRowId = Convert.ToInt32(viewRow.Cells["SlNo"].Value.ToString()) - 1;
                            selectedRowId = viewRow.Index;
                            break;
                        }
                    }

                    CommonDal common = new CommonDal();
                    DataTable dataTable = common.GetItemPriceWithLowestLevel(Convert.ToInt32(txtItemId.Text),
                        Convert.ToInt32(cbItemType.SelectedValue), Convert.ToInt32(cbCustomer.SelectedValue));
                    if (dataTable.Rows.Count < 2)
                    {
                        CommonFunc.DisplayErrorMessage("One or more Item Type is inactive");
                        return;
                    }
                    if (dataTable.Rows[0][2] == DBNull.Value)
                    {
                        CommonFunc.DisplayErrorMessage("Invalid Item Type");
                        return;
                    }
                    int qty = 1;
                    if (editIndex != -1)
                        selectedRowId = 0;
                    if (selectedRowId != -1)
                    {
                        if (editIndex != -1)
                        {
                            qty = (Convert.ToInt32(txtQuantity.Text) * Convert.ToInt32(dataTable.Rows[0][2]));
                            /*
                             SlNo,ItemCode,ItemName,ItemType,Detailed,Quantity,IndividualCost,TotalCost,ItemId,
                             ItemTypeId,PrevQty,ItemPrice,delete,CGST,SGST,IGST,ChangedPrice,SellType,SellQty,StockPosition
                             */
                            dgvItemsList.Rows[editIndex].Cells["ItemCode"].Value = txtItemCode.Text;
                            dgvItemsList.Rows[editIndex].Cells["ItemName"].Value = txtItemName.Text;
                            dgvItemsList.Rows[editIndex].Cells["ItemType"].Value = dataTable.Rows[1][5].ToString();
                            dgvItemsList.Rows[editIndex].Cells["Detailed"].Value = CommonFunc.GetItemTypeDesc(Convert.ToInt32(txtItemId.Text), qty);
                            dgvItemsList.Rows[editIndex].Cells["Quantity"].Value = qty;
                            dgvItemsList.Rows[editIndex].Cells["IndividualCost"].Value = dataTable.Rows[1][1].ToString();
                            dgvItemsList.Rows[editIndex].Cells["TotalCost"].Value = Convert.ToDecimal(txtTotal.Text);
                            dgvItemsList.Rows[editIndex].Cells["ItemId"].Value = txtItemId.Text;
                            dgvItemsList.Rows[editIndex].Cells["ItemTypeId"].Value = dataTable.Rows[1][3].ToString();
                            dgvItemsList.Rows[editIndex].Cells["PrevQty"].Value = txtQuantity.Text;
                            dgvItemsList.Rows[editIndex].Cells["ItemPrice"].Value = dataTable.Rows[1][1].ToString();
                            dgvItemsList.Rows[editIndex].Cells["CGST"].Value = lblItemCGST.Text;
                            dgvItemsList.Rows[editIndex].Cells["SGST"].Value = lblItemSGST.Text;
                            dgvItemsList.Rows[editIndex].Cells["IGST"].Value = lblItemIGST.Text;
                            dgvItemsList.Rows[editIndex].Cells["ChangedPrice"].Value = null;
                            dgvItemsList.Rows[editIndex].Cells["SellType"].Value = cbItemType.SelectedValue.ToString();
                            dgvItemsList.Rows[editIndex].Cells["SellQty"].Value = txtQuantity.Text;
                            dgvItemsList.Rows[editIndex].Cells["BilledPrice"].Value = dataTable.Rows[1][1].ToString();
                            dgvItemsList.Rows[editIndex].Cells["StockPosition"].Value = lblStock.Text;
                            editIndex = -1;
                        }
                        else
                        {
                            if (dgvItemsList.Rows[selectedRowId].Cells["IsBilled"].Value != null)
                            {
                                if (dgvItemsList.Rows[selectedRowId].Cells["ItemId"].Value.ToString() == txtItemId.Text)
                                    if (Convert.ToDecimal(dgvItemsList.Rows[selectedRowId].Cells["IndividualCost"].Value)
                                    != Convert.ToDecimal(dataTable.Rows[1][1]))
                                    {
                                        CommonFunc.DisplayErrorMessage("Item cannot be added as the Item Price has been changed.");
                                        return;
                                    }
                            }
                            qty = ((Convert.ToInt32(txtQuantity.Text) * Convert.ToInt32(dataTable.Rows[0][2])) + Convert.ToInt32(dgvItemsList.Rows[selectedRowId].Cells["Quantity"].Value));
                            dgvItemsList.Rows[selectedRowId].Cells["Quantity"].Value = qty.ToString();
                            dgvItemsList.Rows[selectedRowId].Cells["Detailed"].Value = CommonFunc.GetItemTypeDesc(Convert.ToInt32(txtItemId.Text), qty);
                            decimal ItemCost = Convert.ToDecimal(dgvItemsList.Rows[selectedRowId].Cells["IndividualCost"].Value);
                            decimal totalCost = qty * ItemCost;
                            dgvItemsList.Rows[selectedRowId].Cells["TotalCost"].Value = totalCost;
                            string[] data = dgvItemsList.Rows[selectedRowId].Cells["Detailed"].Value.ToString().Trim().Split(' ');
                            bool isLower = false;
                            int SellQty = 0;
                            for (int i = 0; i < data.Length; i++)
                            {
                                if (i % 2 == 0 && i > 1)
                                {
                                    if (Convert.ToInt32(data[i]) > 0)
                                    {
                                        isLower = false;
                                        break;
                                    }
                                    SellQty = Convert.ToInt32(data[0]);
                                    isLower = true;
                                }
                            }
                            if (isLower)
                            {
                                int? itemType = common.GetItemTypeTop(Convert.ToInt32(txtItemId.Text));
                                if (itemType == null)
                                {
                                    dgvItemsList.Rows[selectedRowId].Cells["Quantity"].Value = "";
                                    dgvItemsList.Rows[selectedRowId].Cells["TotalCost"].Value = "";
                                    CommonFunc.DisplayErrorMessage("Unable to fetch top most item type");
                                    return;
                                }
                                dgvItemsList.Rows[selectedRowId].Cells["SellType"].Value = itemType;
                                dgvItemsList.Rows[selectedRowId].Cells["SellQty"].Value = SellQty;
                            }
                            else
                            {
                                dgvItemsList.Rows[selectedRowId].Cells["SellType"].Value = string.Empty;
                                dgvItemsList.Rows[selectedRowId].Cells["SellQty"].Value = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        /*
                         SlNo,ItemCode,ItemName,ItemType,Detailed,Quantity,IndividualCost,TotalCost,ItemId,
                         ItemTypeId,PrevQty,ItemPrice,delete,CGST,SGST,IGST,ChangedPrice,SellType,SellQty
                         */

                        /*
                         * SlNo,ItemCode,ItemName,ItemType,Detailed,Quantity,IndividualCost,TotalCost,ItemId,
                         * ItemTypeId,PrevQty,ItemPrice,delete,CGST,SGST,IGST,ChangedPrice,SellType,SellQty,Edit,
                         * IsBilled,BilledPrice
                         */
                        qty = (Convert.ToInt32(txtQuantity.Text) * Convert.ToInt32(dataTable.Rows[0][2]));
                        string desc = CommonFunc.GetItemTypeDesc(Convert.ToInt32(txtItemId.Text), qty);

                        dgvItemsList.Rows.Insert(0, new string[] {
                        dgvItemsList.Rows.Count.ToString(),txtItemCode.Text,txtItemName.Text,dataTable.Rows[1][5].ToString(),desc,qty.ToString(),
                            dataTable.Rows[1][1].ToString(),txtTotal.Text,txtItemId.Text,
                            dataTable.Rows[1][3].ToString(),qty.ToString(),dataTable.Rows[1][1].ToString(),null,
                            lblItemCGST.Text,lblItemSGST.Text,lblItemIGST.Text,null,
                            cbItemType.SelectedValue.ToString(),txtQuantity.Text,null,"N",dataTable.Rows[1][1].ToString(),
                            lblStock.Text
                        });
                        //rows++;
                    }

                    updateFinalBill();

                    // after update or insert show all rows (reset any active filter)
                    ShowAllRows();

                    txtItemCode.Text = string.Empty;
                    txtItemName.Text = string.Empty;
                    txtQuantity.Text = string.Empty;
                    txtItemPrice.Text = string.Empty;
                    cbItemType.SelectedIndex = 0;
                    txtItemId.Text = string.Empty;
                    txtTotal.Text = string.Empty;
                    cbItemType.Enabled = false;
                    lblItemCGST.Text = string.Empty;
                    lblItemSGST.Text = string.Empty;
                    lblItemIGST.Text = string.Empty;
                    txtItemCode.Focus();
                }
            }
        }

        private void txtItemCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string q = txtItemCode.Text?.Trim();
                FilterItemsGrid(q);
            }
            catch { }
        }

        private void FilterItemsGrid(string query)
        {
            if (dgvItemsList == null) return;
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    ShowAllRows();
                    return;
                }

                string q = query.ToLowerInvariant();
                bool anyVisible = false;
                foreach (DataGridViewRow row in dgvItemsList.Rows)
                {
                    bool match = false;
                    try
                    {
                        var codeCell = row.Cells["ItemCode"].Value;
                        var nameCell = row.Cells["ItemName"].Value;
                        string code = codeCell == null ? string.Empty : codeCell.ToString().ToLowerInvariant();
                        string name = nameCell == null ? string.Empty : nameCell.ToString().ToLowerInvariant();
                        if (code.Contains(q) || name.Contains(q)) match = true;
                    }
                    catch { }
                    row.Visible = match;
                    if (match) anyVisible = true;
                }
                // if none matched, keep grid empty (no rows visible)
            }
            catch { }
        }

        private void ShowAllRows()
        {
            if (dgvItemsList == null) return;
            try
            {
                foreach (DataGridViewRow row in dgvItemsList.Rows)
                    row.Visible = true;
            }
            catch { }
        }

        private void txtQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtQuantity.Text.Trim()) && !String.IsNullOrEmpty(txtItemPrice.Text.Trim()))
            {
                txtTotal.Text = (Convert.ToDecimal(txtItemPrice.Text) * Convert.ToDecimal(txtQuantity.Text)).ToString();
            }
            else
            {
                txtTotal.Text = string.Empty;
            }
        }

        private bool nonNumberEntered = false;

        private void txtQuantity_KeyDown(object sender, KeyEventArgs e)
        {
            // Initialize the flag to false.
            nonNumberEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (e.KeyCode != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number.
            if (Control.ModifierKeys == Keys.Shift)
            {
                nonNumberEntered = true;
            }
        }

        private void dgvItemsList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn column && e.RowIndex >= 0)
                switch (column.Text.Trim().ToLower())
                {
                    case "edit":
                        editIndex = e.RowIndex;
                        txtItemId.Text = dgvItemsList.Rows[e.RowIndex].Cells["ItemId"].Value.ToString();
                        txtItemCode.Text = dgvItemsList.Rows[e.RowIndex].Cells["ItemCode"].Value.ToString();
                        txtItemName.Text = dgvItemsList.Rows[e.RowIndex].Cells["ItemName"].Value.ToString();
                        txtQuantity.Text = dgvItemsList.Rows[e.RowIndex].Cells["Quantity"].Value.ToString();
                        txtItemPrice.Text = dgvItemsList.Rows[e.RowIndex].Cells["IndividualCost"].Value.ToString();
                        txtTotal.Text = dgvItemsList.Rows[e.RowIndex].Cells["TotalCost"].Value.ToString();
                        CommonFunc.ItemTypeByItem(cbItemType, Convert.ToInt32(txtItemId.Text));
                        cbItemType.SelectedValue = dgvItemsList.Rows[e.RowIndex].Cells["ItemTypeId"].Value.ToString();
                        cbItemType.Enabled = true;
                        lblItemCGST.Text = dgvItemsList.Rows[e.RowIndex].Cells["CGST"].Value.ToString();
                        lblItemSGST.Text = dgvItemsList.Rows[e.RowIndex].Cells["SGST"].Value.ToString();
                        lblItemIGST.Text = dgvItemsList.Rows[e.RowIndex].Cells["IGST"].Value.ToString();
                        break;
                    case "delete":
                        DialogResult result = MessageBox.Show("Are you sure to delete Item : "
                            + dgvItemsList.Rows[e.RowIndex].Cells["ItemName"].Value.ToString(),
                            CommonFunc.ApplicationTitile, MessageBoxButtons.YesNo);
                        try
                        {
                            if (result == DialogResult.Yes)
                            {
                                dgvItemsList.Rows.RemoveAt(dgvItemsList.Rows[e.RowIndex].Index);

                            }
                            updateFinalBill();
                            // ensure any active filter is cleared after delete
                            ShowAllRows();
                        }
                        catch
                        {
                            CommonFunc.DisplayErrorMessage("Unable to remove Item : " + dgvItemsList.Rows[e.RowIndex].Cells["ItemName"].Value.ToString());
                        }
                        break;
                }
        }

        private void dgvItemsList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn &&
                e.RowIndex >= 0)
            {
                if (((DataGridViewTextBoxColumn)senderGrid.Columns[e.ColumnIndex]).HeaderText == "Individual Cost")
                {
                    try
                    {
                        if (Convert.ToDecimal(dgvItemsList.Rows[e.RowIndex].Cells["IndividualCost"].Value) < 0)
                        {
                            CommonFunc.DisplayErrorMessage("Invalid Individual Cost for Item " + dgvItemsList.Rows[e.RowIndex].Cells["ItemName"].Value.ToString());
                            //return;
                        }
                    }
                    catch (Exception)
                    {
                        CommonFunc.DisplayErrorMessage("Invalid Individual Cost for Item " + dgvItemsList.Rows[e.RowIndex].Cells["ItemName"].Value.ToString());
                        dgvItemsList.Rows[e.RowIndex].Cells["IndividualCost"].Value = "";
                        return;
                    }
                    dgvItemsList.Rows[e.RowIndex].Cells["ChangedPrice"].Value = dgvItemsList.Rows[e.RowIndex].Cells["IndividualCost"].Value;
                }
                try
                {
                    if (dgvItemsList.Rows[e.RowIndex].Cells["IsBilled"].Value != null)
                    {
                        if (((DataGridViewTextBoxColumn)senderGrid.Columns[e.ColumnIndex]).HeaderText != "Individual Cost")
                        {
                            if (Convert.ToDecimal(dgvItemsList.Rows[e.RowIndex].Cells["IndividualCost"].Value)
                            != Convert.ToDecimal(dgvItemsList.Rows[e.RowIndex].Cells["ItemPrice"].Value))
                            {
                                if (Convert.ToInt16(dgvItemsList.Rows[e.RowIndex].Cells["PrevQty"].Value)
                                    < Convert.ToInt16(dgvItemsList.Rows[e.RowIndex].Cells["Quantity"].Value))
                                {
                                    dgvItemsList.Rows[e.RowIndex].Cells["Quantity"].Value = dgvItemsList.Rows[e.RowIndex].Cells["PrevQty"].Value;
                                    CommonFunc.DisplayErrorMessage("Item cannot be added as the Item Price has been changed.");
                                    return;
                                }
                            }
                        }
                    }
                    dgvItemsList.Rows[e.RowIndex].Cells["Detailed"].Value = CommonFunc.GetItemTypeDesc(Convert.ToInt32(dgvItemsList.Rows[e.RowIndex].Cells["ItemId"].Value),
                        Convert.ToInt32(dgvItemsList.Rows[e.RowIndex].Cells["Quantity"].Value));

                    decimal ItemCost = Convert.ToDecimal(dgvItemsList.Rows[e.RowIndex].Cells["IndividualCost"].Value);
                    int quantity = Convert.ToInt32(dgvItemsList.Rows[e.RowIndex].Cells["Quantity"].Value);
                    decimal TotalCost = quantity * (ItemCost);

                    dgvItemsList.Rows[e.RowIndex].Cells["TotalCost"].Value = TotalCost;
                    string[] data = dgvItemsList.Rows[e.RowIndex].Cells["Detailed"].Value.ToString().Trim().Split(' ');
                    bool isLower = false;
                    int SellQty = 0;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i % 2 == 0 && i > 1)
                        {
                            if (Convert.ToInt32(data[i]) > 0)
                            {
                                isLower = false;
                                break;
                            }
                            SellQty = Convert.ToInt32(data[0]);
                            isLower = true;
                        }
                    }
                    if (isLower)
                    {
                        CommonDal common = new CommonDal();
                        int? itemType = common.GetItemTypeTop(Convert.ToInt32(dgvItemsList.Rows[e.RowIndex].Cells["ItemId"].Value));
                        if (itemType == null)
                        {
                            dgvItemsList.Rows[e.RowIndex].Cells["Quantity"].Value = "";
                            dgvItemsList.Rows[e.RowIndex].Cells["TotalCost"].Value = "";
                            CommonFunc.DisplayErrorMessage("Unable to fetch top most item type");
                            return;
                        }
                        dgvItemsList.Rows[e.RowIndex].Cells["SellType"].Value = itemType;
                        dgvItemsList.Rows[e.RowIndex].Cells["SellQty"].Value = SellQty;
                    }
                    else
                    {
                        dgvItemsList.Rows[e.RowIndex].Cells["SellType"].Value = string.Empty;
                        dgvItemsList.Rows[e.RowIndex].Cells["SellQty"].Value = string.Empty;
                    }
                }
                catch (Exception)
                {
                    CommonFunc.DisplayErrorMessage("Invalid Quantity for Item " + dgvItemsList.Rows[e.RowIndex].Cells["ItemName"].Value.ToString());
                    dgvItemsList.Rows[e.RowIndex].Cells["Quantity"].Selected = true;
                    return;
                }
                updateFinalBill();
                // after editing a cell, show all rows (reset filter)
                ShowAllRows();
            }
        }

        private void dtgView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvItemsList.CurrentCell.ColumnIndex == dgvItemsList.Columns["IndividualCost"].Index
                & (e.Control != null))
            {
                TextBox tb = (TextBox)e.Control;
                tb.KeyPress += TextBox_KeyPress;
            }
        }

        private void TextBox_KeyPress(Object sender, KeyPressEventArgs e)
        {
            //---if textbox is empty and user pressed a decimal char---
            if (((TextBox)sender).Text == string.Empty & e.KeyChar == (char)46)
            {
                e.Handled = true;
                return;
            }
            //---if textbox already has a decimal point---
            if (((TextBox)sender).Text.Contains(Convert.ToString((char)46)) & e.KeyChar == (char)46)
            {
                e.Handled = true;
                return;
            }
            //if (!(char.IsDigit(e.KeyChar).ToString() ==","))
            if (!(e.KeyChar == 44) & !(e.KeyChar == 45))
            {
                if ((!(char.IsDigit(e.KeyChar) | char.IsControl(e.KeyChar) | (e.KeyChar == (char)46))))
                {
                    e.Handled = true;
                }
            }
        }

        private void rbInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (rbInvoice.Checked)
            {
                btnEstimate.Visible = false;
                btnInvoice.Visible = true;
                gbInvoice.Visible = true;
            }
            updateFinalBill();
        }

        private void rbEstigmate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEstimate.Checked)
            {
                btnEstimate.Visible = true;
                btnInvoice.Visible = false;
                gbInvoice.Visible = false;
            }
            updateFinalBill();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (CommonValidations())
            {
                SaveBill();
            }
        }

        private void cbItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbItemType.SelectedValue == null || String.IsNullOrEmpty(cbItemType.SelectedValue.ToString()))
                return;
            if (!String.IsNullOrEmpty(txtItemId.Text.Trim()) && !String.IsNullOrEmpty(cbCustomer.SelectedValue.ToString()))
            {
                CommonDal common = new CommonDal();
                DataTable dataTable = common.GetItemPrice(Convert.ToInt32(txtItemId.Text), Convert.ToInt32(cbItemType.SelectedValue),
                    Convert.ToInt32(cbCustomer.SelectedValue.ToString()));

                if (dataTable == null)
                {
                    CommonFunc.DisplayErrorMessage("Invalid selections");
                    return;
                }
                txtItemPrice.Text = dataTable.Rows[0][0].ToString();
                if (!String.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    txtTotal.Text = (Convert.ToDecimal(dataTable.Rows[0][0]) * Convert.ToDecimal(txtQuantity.Text)).ToString();
            }
        }

        private bool CommonValidations()
        {
            if (cbCustomer.SelectedIndex == 0)
            {
                CommonFunc.DisplayErrorMessage("Enter Customer Type");
                cbCustomer.Focus();
                return false;
            }
            else if (cbCustomer.SelectedValue.ToString() != "3")
            {
                if (!String.IsNullOrEmpty(txtPhone.Text.Trim()))
                {
                    if (txtPhone.Text.Trim().Length != 10)
                    {
                        CommonFunc.DisplayErrorMessage("Invalid Phone Number");
                        txtPhone.Focus();
                        return false;
                    }
                }
                //if (String.IsNullOrEmpty(txtPhone.Text.Trim()))
                //{
                //    CommonFunc.DisplayErrorMessage("Enter Phone Number");
                //    txtPhone.Focus();
                //    return false;
                //}
                //else
                //{
                //    if (txtPhone.Text.Trim().Length != 10)
                //    {
                //        CommonFunc.DisplayErrorMessage("Invalid Phone Number");
                //        txtPhone.Focus();
                //        return false;
                //    }
                //}
                if (String.IsNullOrEmpty(txtCustomer.Text.Trim()))
                {
                    CommonFunc.DisplayErrorMessage("Enter Customer Name");
                    txtCustomer.Focus();
                    return false;
                }
                if (String.IsNullOrEmpty(txtAddress.Text.Trim()))
                {
                    CommonFunc.DisplayErrorMessage("Enter Address");
                    txtAddress.Focus();
                    return false;
                }
            }
            if (!rbEstimate.Checked && !rbInvoice.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Estimate / Invoice");
                txtPhone.Focus();
                return false;
            }

            //if (String.IsNullOrEmpty(txtAmountPaid.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Enter Amount Paid");
            //    txtAmountPaid.Focus();
            //    return false;
            //}
            if (String.IsNullOrEmpty(txtFinalBill.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Final Bill not calculated");
                txtFinalBill.Focus();
                return false;
            }
            //if (String.IsNullOrEmpty(txtBalance.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Balance Amount not calculated");
            //    txtBalance.Focus();
            //    return false;
            //}

            return true;
        }

        public void SaveBill()
        {
            Bill bill = new Bill
            {
                billedItems = new List<BilledItems>()
            };
            int p = 1;
            try
            {
                foreach (DataGridViewRow row in dgvItemsList.Rows)
                {
                    p++;
                    BilledItems items = new BilledItems()
                    {
                        /*
                            SlNo,ItemCode,ItemName,ItemType,Detailed,Quantity,IndividualCost,TotalCost,ItemId,
                            ItemTypeId,PrevQty
                            */
                        SlNo = Convert.ToInt32(row.Cells["SlNo"].Value),
                        ItemId = Convert.ToInt32(row.Cells["ItemId"].Value),
                        ItemName = row.Cells["ItemName"].Value.ToString(),
                        ItemTypeId = Convert.ToInt32(row.Cells["ItemTypeId"].Value),
                        Quantity = Convert.ToInt32(row.Cells["Quantity"].Value),
                        IndividualCost = Convert.ToDecimal(row.Cells["IndividualCost"].Value),
                        TotalCost = Convert.ToDecimal(row.Cells["TotalCost"].Value),
                        CGST = rbState.Checked ? Convert.ToDecimal(row.Cells["CGST"].Value) : 0,
                        SGST = rbState.Checked ? Convert.ToDecimal(row.Cells["SGST"].Value) : 0,
                        IGST = !rbState.Checked ? Convert.ToDecimal(row.Cells["IGST"].Value) : 0,
                        ChangedPrice = Convert.ToDecimal(row.Cells["ChangedPrice"].Value),
                        SellTypeId = String.IsNullOrEmpty(row.Cells["SellType"].Value.ToString().Trim()) ? 0 : Convert.ToInt32(row.Cells["SellType"].Value),
                        SellQty = String.IsNullOrEmpty(row.Cells["SellQty"].Value.ToString().Trim()) ? 0 : Convert.ToInt32(row.Cells["SellQty"].Value)
                    };

                    bill.billedItems.Add(items);
                }
            }
            catch (Exception)
            {
                CommonFunc.DisplayErrorMessage("Invalid Quantity/Individual Cost in the row " + (p - 1).ToString());
                return;
            }

            if (bill.billedItems.Count == 0)
            {
                CommonFunc.DisplayErrorMessage("Add atleast one item");
                txtItemCode.Focus();
                return;
            }
            var itemDuplicate = bill.billedItems.GroupBy(x => x.ItemName)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (itemDuplicate.Count > 0)
            {
                CommonFunc.DisplayErrorMessage("Duplicate for item('s) " + String.Join(",", itemDuplicate) + " exist in particulars");
                return;
            }
            var PriceZero = bill.billedItems
              .Where(g => g.IndividualCost <= 0)
              .Select(y => y.ItemName)
              .ToList();
            if (bill.billedItems.Count(x => x.IndividualCost <= 0) > 0)
            {
                CommonFunc.DisplayErrorMessage("Duplicate for item('s) " + String.Join(",", PriceZero) + " exist in particulars");
                //CommonFunc.DisplayErrorMessage("Invalid Individual rate exists. Please check.");
                return;
            }
            bill.DMLType = 'I';
            if (!String.IsNullOrEmpty(txtBillId.Text.Trim()))
            {
                bill.BillId = txtBillId.Text;
                bill.DMLType = 'U';
            }
            bill.Name = txtCustomer.Text.Trim();
            bill.Address = txtAddress.Text.Trim();
            if (!String.IsNullOrEmpty(txtPhone.Text.Trim()))
                bill.PhoneNo = Convert.ToInt64(txtPhone.Text.Trim());
            if (!String.IsNullOrEmpty(txtPhone2.Text.Trim()))
                bill.Mobile2 = Convert.ToInt64(txtPhone2.Text.Trim());
            bill.CustomerType = Convert.ToInt32(cbCustomer.SelectedValue);
            bill.IsEstimateInvoice = rbEstimate.Checked ? 'E' : 'I';
            bill.BillAmount = Convert.ToDecimal(txtBillAmt.Text.Trim());
            if (!String.IsNullOrEmpty(txtDiscPerItem.Text))
                bill.DiscountPerItem = Convert.ToDecimal(txtDiscPerItem.Text.Trim());
            if (!String.IsNullOrEmpty(txtDiscPercentage.Text))
                bill.DiscountPercentage = Convert.ToDecimal(txtDiscPercentage.Text.Trim());
            if (!String.IsNullOrEmpty(lblBillNo.Text))
                bill.BillNo = lblBillNo.Text.Trim();
            bill.IsInState = rbState.Checked ? true : false;
            if (!String.IsNullOrEmpty(txtGSTIN.Text))
                bill.CustGSTIN = txtGSTIN.Text.Trim();
            if (!String.IsNullOrEmpty(txtFinalBill.Text))
                bill.TotalBillAmount = Convert.ToDecimal(txtFinalBill.Text.Trim());
            //if (!String.IsNullOrEmpty(txtAddl.Text))
            //    bill.AddlDisc = Convert.ToDecimal(txtAddl.Text.Trim());
            //if (!String.IsNullOrEmpty(lblAddlAmt.Text))
            //    bill.TotalFinalAmount = Convert.ToDecimal(lblAddlAmt.Text.Trim());
            //if (!String.IsNullOrEmpty(txtAdd.Text))
            //    bill.AddDesc = txtAdd.Text.Trim();
            if (!String.IsNullOrEmpty(txtInvoice.Text))
                bill.InvoiceNo = txtInvoice.Text.Trim();

            if (!String.IsNullOrEmpty(txtPincode.Text))
                bill.Pincode = txtPincode.Text.Trim();
            if (!String.IsNullOrEmpty(txtPOS.Text))
                bill.POS = txtPOS.Text.Trim();
            if (!String.IsNullOrEmpty(txtStCd.Text))
                bill.StateCode = txtStCd.Text.Trim();
            if (!String.IsNullOrEmpty(txtLocation.Text))
                bill.Location = txtLocation.Text.Trim();

            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            BillDal billDal = new BillDal();
            billDal.SaveBills(bill, alertMsgInfo);
            //alertMsgInfo.SaveStatus = true;
            //alertMsgInfo.Message = "Saved successfully.";

            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.SaveMessage(alertMsgInfo.Message);
                txtBillId.Text = string.Empty;
                switch (bill.IsEstimateInvoice)
                {
                    case 'E':
                        if (rbA4.Checked)
                            PrintEstimate(alertMsgInfo.Id, cbDetails.Checked);
                        else
                            PrintRetail(alertMsgInfo.Id);
                        break;
                    case 'I':
                        if (rbA4.Checked)
                        {
                            if (rbState.Checked)
                                PrintInvoice(alertMsgInfo.Id);
                            else
                                PrintInvoiceNational(alertMsgInfo.Id);
                        }
                        else
                            PrintRetail(alertMsgInfo.Id);
                        break;
                }
                //try
                //{
                //    if (SMSClass.IsRelease && bill.PhoneNo != null && String.IsNullOrEmpty(txtBillId.Text.Trim()))
                //        SMSClass.GetPageContent(Convert.ToInt64(bill.PhoneNo), alertMsgInfo.Id, bill.BillAmount);
                //}
                //catch (Exception)
                //{
                //    CommonFunc.DisplayErrorMessage("Unable to send SMS. Please check internet/SMS Credits.");
                //}
                CleanForm();
            }
            else
            {
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
            }
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            if (!CommonValidations())
            {
                return;
            }
            //if (String.IsNullOrEmpty(txtDiscPerItem.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Enter Discount per Item");
            //    txtDiscPerItem.Focus();
            //    return;
            //}
            //if (String.IsNullOrEmpty(txtDiscPercentage.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Enter Discount percentage");
            //    txtDiscPercentage.Focus();
            //    return;
            //}
            //if (String.IsNullOrEmpty(txtBillNo.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Enter Bill No");
            //    txtBillNo.Focus();
            //    return;
            //}
            SaveBill();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            CleanForm();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtBillId.Text))
            {
                CommonFunc.DisplayErrorMessage("Select Invoice from View Bills");
                return;
            }
            BillDal dal = new BillDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            dal.RemoveInvoice(Convert.ToInt64(txtBillId.Text), alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CleanForm();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
                btnRemove.Visible = false;
            }
            else
            {
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtBillId.Text))
            {
                CommonFunc.DisplayErrorMessage("Select Invoice from View Bills");
                return;
            }
            BillDal dal = new BillDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            dal.CancelInvoice(Convert.ToInt64(txtBillId.Text), alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CleanForm();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
                btnCancel.Visible = false;
            }
            else
            {
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
            }
        }

        private void txtDiscPerItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtDiscPerItem);
        }

        private void txtDiscPerItem_TextChanged(object sender, EventArgs e)
        {
            updateFinalBill();
            updateFinal();
        }

        private void txtDiscPercentage_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtDiscPercentage);
        }

        private void txtDiscPercentage_TextChanged(object sender, EventArgs e)
        {
            updateFinalBill();
        }

        //private void txtAmountPaid_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    e.Handled = CommonFunc.IsDecimal(e, txtAmountPaid);
        //}

        private void updateFinalBill()
        {
            decimal billAmount = 0;
            decimal discPerItem = 0;
            if (!String.IsNullOrEmpty(txtDiscPerItem.Text.Trim()) && txtDiscPerItem.Text.Trim() != "-")
                discPerItem = Convert.ToDecimal(txtDiscPerItem.Text);
            if (discPerItem > 100)
            {
                CommonFunc.DisplayErrorMessage("Discount per Item is exceeded 100 percent");
                txtDiscPerItem.Text = "";
                return;
            }
            decimal cgstAmt = 0, sgstAmt = 0, igstAmt = 0;
            int i = dgvItemsList.Rows.Count;
            decimal discPercentage = !String.IsNullOrEmpty(txtDiscPercentage.Text.Trim()) ? Convert.ToDecimal(txtDiscPercentage.Text) : 0;
            foreach (DataGridViewRow row in dgvItemsList.Rows)
            {
                //row.Cells["SlNo"].Value = i.ToString();
                //i++;
                row.Cells["SlNo"].Value = i--;
                if (row.Cells["ChangedPrice"].Value == null || Convert.ToDecimal(row.Cells["ChangedPrice"].Value) == 0)
                {
                    row.Cells["IndividualCost"].Value = Math.Round(Convert.ToDecimal(row.Cells["BilledPrice"].Value) -
                            (discPerItem * Convert.ToDecimal(row.Cells["BilledPrice"].Value) / 100), 2);
                    //if (row.Cells["IsBilled"].Value == null || row.Cells["IsBilled"].Value.ToString() == "")
                    //    row.Cells["IndividualCost"].Value = Math.Round(Convert.ToDecimal(row.Cells["ItemPrice"].Value) -
                    //        (discPerItem * Convert.ToDecimal(row.Cells["ItemPrice"].Value) / 100), 2);
                    //else
                    //{
                    //}
                }
                else
                    row.Cells["IndividualCost"].Value = Math.Round(Convert.ToDecimal(row.Cells["ChangedPrice"].Value) -
                        (discPerItem * Convert.ToDecimal(row.Cells["ChangedPrice"].Value) / 100), 2);

                row.Cells["TotalCost"].Value = Math.Round(Convert.ToDecimal(row.Cells["Quantity"].Value) *
                    Convert.ToDecimal(row.Cells["IndividualCost"].Value), 2);
                billAmount += Convert.ToDecimal(row.Cells["TotalCost"].Value);
                if (rbInvoice.Checked)
                {
                    decimal cgst = Convert.ToDecimal(row.Cells["CGST"].Value);
                    decimal sgst = Convert.ToDecimal(row.Cells["SGST"].Value);
                    decimal igst = Convert.ToDecimal(row.Cells["IGST"].Value);
                    decimal TotalCost = Convert.ToDecimal(row.Cells["TotalCost"].Value);
                    TotalCost -= (TotalCost * discPercentage / 100);

                    if (rbState.Checked)
                    {
                        cgstAmt += Math.Round(cgst * TotalCost / 100, 2);
                        sgstAmt += Math.Round(sgst * TotalCost / 100, 2);
                    }
                    else
                        igstAmt += Math.Round(igst * TotalCost / 100, 2);
                }
            }
            //Console.WriteLine(Math.Round(billAmount, 2).ToString());
            txtBillAmt.Text = Math.Round(billAmount, 2).ToString();
            lblCGST.Text = cgstAmt.ToString();
            lblSGST.Text = sgstAmt.ToString();
            lblIGST.Text = igstAmt.ToString();

            lblTaxableAmt.Text = Math.Round(billAmount - (discPercentage * billAmount / 100), 2).ToString();
            txtFinalBill.Text = Math.Round(Convert.ToDecimal(lblTaxableAmt.Text) + cgstAmt + sgstAmt + igstAmt, 2).ToString();
            //updateFinal();
        }
        private void updateFinalBillOld()
        {
            decimal billAmount = 0;
            decimal discPerItem = 0;
            if (!String.IsNullOrEmpty(txtDiscPerItem.Text.Trim()) && txtDiscPerItem.Text.Trim() != "-")
                discPerItem = Convert.ToDecimal(txtDiscPerItem.Text);
            if (discPerItem > 100)
            {
                CommonFunc.DisplayErrorMessage("Discount per Item is exceeded 100 percent");
                txtDiscPerItem.Text = "";
                return;
            }
            decimal cgstAmt = 0, sgstAmt = 0, igstAmt = 0;
            int i = dgvItemsList.Rows.Count;
            decimal discPercentage = !String.IsNullOrEmpty(txtDiscPercentage.Text.Trim()) ? Convert.ToDecimal(txtDiscPercentage.Text) : 0;
            foreach (DataGridViewRow row in dgvItemsList.Rows)
            {
                //row.Cells["SlNo"].Value = i.ToString();
                //i++;
                row.Cells["SlNo"].Value = i--;
                //if (row.Cells["ChangedPrice"].Value == null || Convert.ToDecimal(row.Cells["ChangedPrice"].Value) == 0)
                //{
                //    if (row.Cells["IsBilled"].Value == null || row.Cells["IsBilled"].Value.ToString() == "")
                //        row.Cells["IndividualCost"].Value = Convert.ToDecimal(row.Cells["ItemPrice"].Value) -
                //        (discPerItem * Convert.ToDecimal(row.Cells["ItemPrice"].Value) / 100);
                //}
                //else
                row.Cells["IndividualCost"].Value = Convert.ToDecimal(row.Cells["ItemPrice"].Value) -
                    (discPerItem * Convert.ToDecimal(row.Cells["ItemPrice"].Value) / 100);

                row.Cells["TotalCost"].Value = Convert.ToDecimal(row.Cells["Quantity"].Value) *
                    Convert.ToDecimal(row.Cells["IndividualCost"].Value);
                billAmount += Convert.ToDecimal(row.Cells["TotalCost"].Value);
                if (rbInvoice.Checked)
                {
                    decimal cgst = Convert.ToDecimal(row.Cells["CGST"].Value);
                    decimal sgst = Convert.ToDecimal(row.Cells["SGST"].Value);
                    decimal igst = Convert.ToDecimal(row.Cells["IGST"].Value);
                    decimal TotalCost = Convert.ToDecimal(row.Cells["TotalCost"].Value);
                    TotalCost -= (TotalCost * discPercentage / 100);

                    if (rbState.Checked)
                    {
                        cgstAmt += Math.Round(cgst * TotalCost / 100, 2);
                        sgstAmt += Math.Round(sgst * TotalCost / 100, 2);
                    }
                    else
                        igstAmt += Math.Round(igst * TotalCost / 100, 2);
                }
            }

            txtBillAmt.Text = Math.Round(billAmount, 2).ToString();
            lblCGST.Text = cgstAmt.ToString();
            lblSGST.Text = sgstAmt.ToString();
            lblIGST.Text = igstAmt.ToString();

            lblTaxableAmt.Text = Math.Round(billAmount - (discPercentage * billAmount / 100), 2).ToString();
            txtFinalBill.Text = Math.Round(Convert.ToDecimal(lblTaxableAmt.Text) + cgstAmt + sgstAmt + igstAmt, 2).ToString();
            //updateFinal();
        }

        private void txtCGST_TextChanged(object sender, EventArgs e)
        {
            updateFinalBill();
        }

        private void txtBillAmt_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDiscPercentage.Text.Trim()) && txtDiscPercentage.Text.Trim() != "-"
                && !String.IsNullOrEmpty(txtBillAmt.Text))
                lblTaxableAmt.Text = Math.Round(Convert.ToDecimal(txtBillAmt.Text) - (Convert.ToDecimal(txtDiscPercentage.Text) * Convert.ToDecimal(txtBillAmt.Text) / 100), 2).ToString();
            else
                lblTaxableAmt.Text = "0.00";
            //if (!String.IsNullOrEmpty(txtCGST.Text.Trim()) && txtCGST.Text.Trim() != "-")
            //    lblCGST.Text = Math.Round((Convert.ToDecimal(txtCGST.Text) * Convert.ToDecimal(lblTaxableAmt.Text) / 100), 2).ToString();
            //else
            //    lblCGST.Text = "0.00";
            //if (!String.IsNullOrEmpty(txtSGST.Text.Trim()) && txtSGST.Text.Trim() != "-")
            //    lblSGST.Text = Math.Round((Convert.ToDecimal(txtSGST.Text) * Convert.ToDecimal(lblTaxableAmt.Text) / 100), 2).ToString();
            //else
            //    lblSGST.Text = "0.00";
            //if (!String.IsNullOrEmpty(txtIGST.Text.Trim()) && txtIGST.Text.Trim() != "-")
            //    lblIGST.Text = Math.Round((Convert.ToDecimal(txtIGST.Text) * Convert.ToDecimal(lblTaxableAmt.Text) / 100), 2).ToString();
            //else
            //    lblIGST.Text = "0.00";
            updateFinalBill();
        }

        private void lblTaxableAmt_TextChanged(object sender, EventArgs e)
        {
            //if (!String.IsNullOrEmpty(txtCGST.Text.Trim()) && txtCGST.Text.Trim() != "-")
            //    lblCGST.Text = Math.Round((Convert.ToDecimal(txtCGST.Text) * Convert.ToDecimal(lblTaxableAmt.Text) / 100), 2).ToString();
            //else
            //    lblCGST.Text = "0.00";
            //if (!String.IsNullOrEmpty(txtSGST.Text.Trim()) && txtSGST.Text.Trim() != "-")
            //    lblSGST.Text = Math.Round((Convert.ToDecimal(txtSGST.Text) * Convert.ToDecimal(lblTaxableAmt.Text) / 100), 2).ToString();
            //else
            //    lblSGST.Text = "0.00";
            //if (!String.IsNullOrEmpty(txtIGST.Text.Trim()) && txtIGST.Text.Trim() != "-")
            //    lblIGST.Text = Math.Round((Convert.ToDecimal(txtIGST.Text) * Convert.ToDecimal(lblTaxableAmt.Text) / 100), 2).ToString();
            //else
            //    lblIGST.Text = "0.00";
        }

        private void rbState_CheckedChanged(object sender, EventArgs e)
        {
            updateFinalBill();
        }

        private void rbOut_CheckedChanged(object sender, EventArgs e)
        {
            updateFinalBill();
        }

        //private void txtAmountPaid_TextChanged(object sender, EventArgs e)
        //{
        //    updateBalance();
        //}

        //private void txtFinalBill_TextChanged(object sender, EventArgs e)
        //{
        //    updateBalance();
        //}
        //private void updateBalance()
        //{
        //    if (!String.IsNullOrEmpty(txtFinalBill.Text.Trim()))
        //        txtBalance.Text = (Convert.ToDecimal(txtFinalBill.Text) - (String.IsNullOrEmpty(txtAmountPaid.Text) ? 0 : Convert.ToDecimal(txtAmountPaid.Text))).ToString();
        //    else
        //        txtBalance.Text = "0.00";
        //}
        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtPhone.Text))
                e.Handled = true;
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            if (txtPhone.Text.Length == 10)
            {
                CommonDal common = new CommonDal();
                CustomerDetails customer = common.GetCustomerDetails(Convert.ToInt64(txtPhone.Text));
                if (customer != null)
                {
                    txtCustomer.Text = customer.Name;
                    txtAddress.Text = customer.Address;
                    //txtCusType.Text = customer.CustomerType.ToString();
                    txtItemCode.Focus();
                }
                else
                {
                    txtCustomer.Text = string.Empty;
                    txtAddress.Text = string.Empty;
                    //txtCusType.Text = string.Empty;
                    //txtCusType.Enabled = true;
                    //txtCusType.Focus();
                }
            }
        }

        public void PrintEstimate(Int64 BillId, bool ShowDetails)
        {
            Reports.Estimate estimate = new Reports.Estimate();
            BillDal billDal = new BillDal();
            DataSet dataSet = billDal.GetBillDetailsByIdRpt(BillId);
            estimate.Database.Tables["USP_RPT_GET_BillDetailsById"].SetDataSource(dataSet.Tables[0]);
            estimate.Database.Tables["USP_GET_ShotDetails"].SetDataSource(ShopDetails.dtShopDetails);

            estimate.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
            estimate.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;

            List<ReportParameters> reportParameters = new List<ReportParameters>
            {
                new ReportParameters() { ParamName = "Details", ParamValue = ShowDetails }
            };

            using (ViewReport report = new ViewReport(estimate, reportParameters))
            {
                report.Text = "View Estimate";
                report.ShowDialog();
            }
        }

        public void PrintInvoice(Int64 BillId)
        {
            Reports.TaxInvoiceUpdated estimate = new Reports.TaxInvoiceUpdated();
            BillDal billDal = new BillDal();
            DataSet dataSet = billDal.GetInvoiceBillDetailsById(BillId);
            estimate.Database.Tables["USP_RPT_GET_InvoiceBillDetailsById"].SetDataSource(dataSet.Tables[0]);
            estimate.Database.Tables["USP_GET_ShotDetails"].SetDataSource(ShopDetails.dtShopDetails);

            estimate.Subreports[0].Database.Tables["FW_Invoice_Items"].SetDataSource(dataSet.Tables[1]);

            estimate.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
            estimate.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            //estimate.PrintToPrinter(1, false, 0, 0);
            using (ViewReport report = new ViewReport(estimate))
            {
                report.Text = "View Invoice";
                report.ShowDialog();
            }
        }

        public void PrintInvoiceNational(Int64 BillId)
        {
            Reports.TaxInvoiceIGST estimate = new Reports.TaxInvoiceIGST();
            BillDal billDal = new BillDal();
            DataSet dataSet = billDal.GetInvoiceBillDetailsById(BillId);
            estimate.Database.Tables["USP_RPT_GET_InvoiceBillDetailsById"].SetDataSource(dataSet.Tables[0]);
            estimate.Database.Tables["USP_GET_ShotDetails"].SetDataSource(ShopDetails.dtShopDetails);

            estimate.Subreports[0].Database.Tables["FW_Invoice_Items"].SetDataSource(dataSet.Tables[1]);

            estimate.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
            estimate.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            //estimate.PrintToPrinter(1, false, 0, 0);
            using (ViewReport report = new ViewReport(estimate))
            {
                report.Text = "View Invoice";
                report.ShowDialog();
            }
        }

        public void PrintRetail(Int64 BillId)
        {
            try
            {
                Reports.Retail estimate = new Reports.Retail();
                BillDal billDal = new BillDal();
                DataSet dataSet = billDal.GetBillDetailsByIdRpt(BillId);
                estimate.Database.Tables["USP_RPT_GET_BillDetailsById"].SetDataSource(dataSet.Tables[0]);
                estimate.Database.Tables["USP_GET_ShotDetails"].SetDataSource(ShopDetails.dtShopDetails);

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[1] {
                    new DataColumn("PrintType", typeof(string))
                });
                dt.Rows.Add("Customer");
                estimate.Database.Tables["PrintType"].SetDataSource(dt);

                using (ViewReport report = new ViewReport(estimate))
                {
                    report.Text = "View Estimate";
                    report.ShowDialog();
                }

                dt.Rows.RemoveAt(0);
                estimate.Database.Tables.Reset();
                estimate.Database.Tables["USP_RPT_GET_BillDetailsById"].SetDataSource(dataSet.Tables[0]);
                estimate.Database.Tables["USP_GET_ShotDetails"].SetDataSource(ShopDetails.dtShopDetails);
                dt.Rows.Add("Company");
                estimate.Database.Tables["PrintType"].SetDataSource(dt);

                //estimate.PrintToPrinter(1, false, 0, 0);
                using (ViewReport report = new ViewReport(estimate))
                {
                    report.Text = "View Estimate";
                    report.ShowDialog();
                }
            }
            catch (TypeInitializationException er)
            {
                Errorlog.WriteToErrorLog(er, UserSessionEntity.UserID, null, "Class : Billing, Method : PrintEstimate");
                CommonFunc.DisplayErrorMessage("Unable to connect printer or print. " + Environment.NewLine + er.Message);
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, null, "Class : Billing, Method : PrintRetail");
                CommonFunc.DisplayErrorMessage("Unable to connect or print on Thermal Printer");
            }
        }

        private void Billing_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dgvItemsList.Rows.Count > 0)
            {
                if (MessageBox.Show(this, "Do you want to save the current bill?", CommonFunc.ApplicationTitile,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Cancel the Closing event from closing the form.
                    e.Cancel = true;
                    // Call method to save file...
                }
            }
        }

        private void CleanForm()
        {
            if (dgvItemsList.DataSource == null)
                dgvItemsList.Rows.Clear();
            else
            {
                List<BilledItems> billedItems = new List<BilledItems>();
                dgvItemsList.DataSource = billedItems;
            }
            DisplayGrid(dgvItemsList);
            txtBillId.Text = string.Empty;
            txtCustomer.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPhone.Text = string.Empty;
            cbCustomer.SelectedIndex = 0;
            cbCustomer.Enabled = true;
            cbCustomer.Visible = true;
            txtBillAmt.Text = string.Empty;
            txtTotal.Text = string.Empty;
            txtDiscPerItem.Text = string.Empty;
            txtDiscPercentage.Text = string.Empty;
            lblBillNo.Text = string.Empty;
            lblTaxableAmt.Text = "0.00";
            //txtCGST.Text = string.Empty;
            //lblCGST.Text = "0.00";
            //txtSGST.Text = string.Empty;
            //lblSGST.Text = "0.00";
            //txtIGST.Text = string.Empty;
            //lblIGST.Text = "0.00";
            //txtHSNCode.Text = string.Empty;
            txtGSTIN.Text = string.Empty;
            txtFinalBill.Text = string.Empty;
            //txtAmountPaid.Text = string.Empty;
            //txtBalance.Text = string.Empty;
            rbEstimate.Checked = true;
            rbEstimate.Enabled = true;
            rbState.Checked = true;
            btnRemove.Visible = false;
            btnCancel.Visible = false;
            //rows = 1;

            //rbA4.Visible = false;
            //rbThermal.Visible = false;
            this.Text = "Billing";
            //txtAddl.Text = string.Empty;
            //lblAddlAmt.Text = string.Empty;
            //txtAddl.Enabled = true;
            txtPhone2.Text = string.Empty;
            cbDetails.Checked = false;

            txtPOS.Text = string.Empty;
            txtStCd.Text = string.Empty;
            txtPincode.Text = string.Empty;
            txtLocation.Text = string.Empty;
        }

        private void StopSorting()
        {
            foreach (DataGridViewColumn column in dgvItemsList.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void cbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtItemCode.Enabled = false;
            if (!String.IsNullOrEmpty(cbCustomer.SelectedValue.ToString()))
            {
                //cbCustomer.Visible = false;
                cbCustomer.Enabled = false;
                switch (Convert.ToInt32(cbCustomer.SelectedValue))
                {
                    case 1:
                        this.Text = "Alpha";
                        txtItemCode.Enabled = true;
                        customerType = Convert.ToInt32(cbCustomer.SelectedValue);
                        rbA4.Checked = true;
                        //rbThermal.Visible = false;
                        break;
                    case 2:
                        this.Text = "Beta";
                        txtItemCode.Enabled = true;
                        customerType = Convert.ToInt32(cbCustomer.SelectedValue);
                        rbA4.Checked = true;
                        //rbThermal.Visible = false;
                        break;
                    case 3:
                        this.Text = "Gamma";
                        txtItemCode.Enabled = true;
                        customerType = Convert.ToInt32(cbCustomer.SelectedValue);
                        rbA4.Visible = true;
                        //rbThermal.Visible = true;
                        rbThermal.Checked = true;
                        break;
                    default:
                        cbCustomer.SelectedIndex = 0;
                        customerType = 0;
                        CommonFunc.DisplayErrorMessage("Invalid Customer Type");
                        break;
                }

                if (cbCustomer.SelectedIndex > 0)
                {
                    CommonDal common = new CommonDal();
                    foreach (DataGridViewRow row in dgvItemsList.Rows)
                    {
                        DataTable dataTable = common.GetItemPriceWithLowestLevel(Convert.ToInt32(row.Cells["ItemId"].Value),
                        Convert.ToInt32(row.Cells["ItemTypeId"].Value), Convert.ToInt32(cbCustomer.SelectedValue));
                        if (dataTable.Rows.Count < 2)
                        {
                            CommonFunc.DisplayErrorMessage("One or more Item Type is inactive for item : " + row.Cells["ItemName"].Value);
                            return;
                        }
                        if (dataTable.Rows[0][2] == DBNull.Value)
                        {
                            CommonFunc.DisplayErrorMessage("Invalid Item Type");
                            return;
                        }
                        int qty = (Convert.ToInt32(dgvItemsList.Rows[row.Index].Cells["Quantity"].Value) * Convert.ToInt32(dataTable.Rows[0][2]));
                        dgvItemsList.Rows[row.Index].Cells["Quantity"].Value = qty;
                        dgvItemsList.Rows[row.Index].Cells["IndividualCost"].Value = dataTable.Rows[1][1].ToString();
                        dgvItemsList.Rows[row.Index].Cells["TotalCost"].Value = qty * Convert.ToDecimal(dataTable.Rows[1][1]);
                        dgvItemsList.Rows[row.Index].Cells["ItemTypeId"].Value = dataTable.Rows[1][3].ToString();
                        dgvItemsList.Rows[row.Index].Cells["PrevQty"].Value = qty;
                        dgvItemsList.Rows[row.Index].Cells["ItemPrice"].Value = dataTable.Rows[1][1].ToString();
                        dgvItemsList.Rows[row.Index].Cells["ChangedPrice"].Value = null;
                        dgvItemsList.Rows[row.Index].Cells["SellType"].Value = dataTable.Rows[1][3].ToString();
                        dgvItemsList.Rows[row.Index].Cells["SellQty"].Value = qty;
                    }
                    updateFinalBill();
                }
            }
        }
        /*
        private void cbAdditional_CheckedChanged(object sender, EventArgs e)
        {
            gbAddl.Visible = cbAdditional.Checked;
            if (!cbAdditional.Checked)
            {
                txtAddl.Text = string.Empty;
                lblAddlAmt.Text = string.Empty;
            }
        }

        private void txtAddl_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtAddl.Text))
            {
                lblAddlAmt.Text = string.Empty;
                return;
            }
            if (Convert.ToDecimal(txtAddl.Text) < 0)
            {
                CommonFunc.DisplayErrorMessage("Invalid Additional percentage");
                return;
            }
            if (Convert.ToDecimal(txtAddl.Text) > 100)
            {
                CommonFunc.DisplayErrorMessage("Additional percentage exceeded 100");
            }
            updateFinal();
        }

        private void txtAddl_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtAddl);
        }
        */
        private void updateFinal()
        {
            if (String.IsNullOrEmpty(txtBillAmt.Text))
            {
                CommonFunc.DisplayErrorMessage("Bill Amount is blank");
                return;
            }

            decimal billAmount = Convert.ToDecimal(txtBillAmt.Text);
            //decimal addldisc = !String.IsNullOrEmpty(txtAddl.Text.Trim()) ? Convert.ToDecimal(txtAddl.Text) : 0;

            //lblAddlAmt.Text = Math.Round(billAmount * addldisc / 100, 2).ToString();
            txtFinalBill.Text = Math.Round(billAmount, 2).ToString();
            if (rbInvoice.Checked)
            {
                lblTaxableAmt.Text = Math.Round(billAmount, 2).ToString();
                updateFinalBill();
            }
        }

        private void txtPhone2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtPhone2.Text))
                e.Handled = true;
        }
        private void txtPincode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtPincode.Text))
                e.Handled = true;
        }

        private void dgvItemsList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dgvItemsList.Rows)
            {
                Myrow.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
                if (Myrow.Cells["StockPosition"].Value != null)
                {
                    try
                    {
                        var stockText = Myrow.Cells["StockPosition"].Value.ToString().Trim();
                        if (!string.IsNullOrEmpty(stockText))
                        {
                            var parts = stockText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 0)
                            {
                                var first = parts[0].Trim();
                                if (int.TryParse(first, out int stockVal))
                                {
                                    if (stockVal >= 0 && stockVal <= 5)
                                        Myrow.DefaultCellStyle.ForeColor = Color.Orange;
                                    else if (stockVal < 0)
                                        Myrow.DefaultCellStyle.ForeColor = Color.Red;
                                    else
                                        Myrow.DefaultCellStyle.ForeColor = dgvItemsList.DefaultCellStyle.ForeColor;
                                }
                                else
                                {
                                    // first token is not an integer (could be '-' or textual). Leave default color.
                                    Myrow.DefaultCellStyle.ForeColor = dgvItemsList.DefaultCellStyle.ForeColor;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // In case of any unexpected format, fallback to default color to avoid exceptions
                        Myrow.DefaultCellStyle.ForeColor = dgvItemsList.DefaultCellStyle.ForeColor;
                    }
                }
            }
        }

    }
}
