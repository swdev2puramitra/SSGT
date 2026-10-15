using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using FireWorksEntity.Masters;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FireWorks.Masters
{
    public partial class Items : Form
    {
        bool isEdit = false;
        public bool fromForm { get; set; }//true  -->  ViewInventory, false  -->  Inactive
        public Items()
        {
            InitializeComponent();
            // show loading overlay while form initializes
            FireWorks.UI.UiHelpers.ShowLoading(this, "Loading items...");
            this.Text = CommonFunc.ApplicationTitile;
            try
            {
                // apply styling helpers at runtime for this form
                FireWorks.UI.UiHelpers.ApplyFormStyle(this);
            }
            catch { }
            bindCombo();
            //gbItem.Visible = false;
            lblItemId.Text = string.Empty;
            // hide loading overlay once initialization completes
            FireWorks.UI.UiHelpers.HideLoading(this);
        }

        public Items(int ItemId)
        {
            // show loading overlay while form initializes
            FireWorks.UI.UiHelpers.ShowLoading(this, "Loading items...");
            this.Text = CommonFunc.ApplicationTitile;
            try
            {
                // apply styling helpers at runtime for this form
                FireWorks.UI.UiHelpers.ApplyFormStyle(this);
            }
            catch { }
            InitializeComponent();
            bindCombo();
            //gbItem.Visible = true;
            LoadItemDetails(ItemId);
            isEdit = true;
            // hide loading overlay once initialization completes
            FireWorks.UI.UiHelpers.HideLoading(this);
        }

        private void bindCombo()
        {
            CommonDal dal = new CommonDal();
            CommonFunc.CategoryFill(cbCategory);
            CommonFunc.SubCategoryFill(cbSubCategory, null, true);
            CommonFunc.CompaniesFill(cbCompany);
            CommonFunc.ItemTypeFill(cbItemType);
            CommonFunc.HSNFill(cbHSN);

            List<ConversionsEntity> conversions = dal.GetConversions(true);
            conversions.Insert(0, new ConversionsEntity { ConversionId = null, ConversionName = "--Select Conversion--" });
            cbConversion.DataSource = conversions;

            txtItemCode.Focus();
        }

        private void LoadItemDetails(int ItemId)
        {
            CommonDal common = new CommonDal();
            ItemsEntity entity = common.GetItem(ItemId);
            if (entity != null)
            {
                //gbItem.Visible = true;
                lblItemId.Text = ItemId.ToString();
                txtItemCode.Text = entity.ItemCode;
                txtItemName.Text = entity.ItemName;
                cbCompany.SelectedValue = entity.CompanyId;
                cbItemType.SelectedValue = entity.ItemType;
                cbCategory.SelectedValue = entity.CategoryId;
                cbSubCategory.SelectedValue = entity.SubCategoryId;
                txtWholeSale.Text = entity.WholeSalePrice.ToString();
                txtSemi.Text = entity.SemiWholeSalePrice.ToString();
                txtRetail.Text = entity.RetailPrice.ToString();
                cbConversion.SelectedValue = entity.ConversionId;
                cbHSN.SelectedValue = entity.HSNId;
                rbActive.Checked = entity.IsActive;
                rbInactive.Checked = !entity.IsActive;
                lblSemi.Text = entity.SemiWholeSaleMargin.ToString();
                lblRetail.Text = entity.RetailMargin.ToString();
                if (dgvConversions.Rows.Count > 0)
                    dgvConversions.Rows.Clear();
                foreach (ItemConversionQuantity itemType in entity.quantities)
                {
                    dgvConversions.Rows.Add(new string[] { itemType.ItemTypeId.ToString(),
                        itemType.ItemTypeName.ToString(),
                        itemType.Quantity.ToString() ,
                        itemType.WholeSale.ToString(),
                        itemType.SemiWholeSale.ToString(),
                        itemType.Retail.ToString()});
                }
                if (dgvConversions.Rows.Count > 0)
                    dgvConversions.Rows[0].Cells[2].ReadOnly = true;
                if (UserSessionEntity.RoleId == 1)
                    btnSave.Visible = true;
                else
                    btnSave.Visible = false;
            }
            else
            {
                CommonFunc.DisplayErrorMessage("Unable to get the details");
            }
        }

        private void cbConversion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvConversions.Rows.Count > 0)
                dgvConversions.Rows.Clear();
            dgvConversions.Refresh();
            if (cbConversion.SelectedValue != null)
            {
                if (!String.IsNullOrEmpty(cbConversion.SelectedValue.ToString()))
                {
                    CommonDal dal = new CommonDal();
                    List<ItemTypeEntity> itemTypes = dal.GetConversionItems(Convert.ToInt32(cbConversion.SelectedValue));
                    if (itemTypes != null)
                    {
                        int i = 0;
                        foreach (ItemTypeEntity itemType in itemTypes)
                        {
                            string quantity = "";
                            if (i == 0)
                            {
                                quantity = "1";
                                i++;
                            }
                            dgvConversions.Rows.Add(new string[] { itemType.ItemTypeId.ToString(), itemType.ItemTypeName.ToString(), quantity });
                        }

                        dgvConversions.Rows[0].Cells[2].ReadOnly = true;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtItemCode.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Item Code");
                txtItemCode.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtItemName.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Item Name");
                txtItemName.Focus();
                return;
            }
            if (cbCompany.SelectedIndex <= 0)
            {
                CommonFunc.DisplayErrorMessage("Select Company Name");
                cbCompany.Focus();
                return;
            }
            if (cbItemType.SelectedIndex <= 0)
            {
                CommonFunc.DisplayErrorMessage("Select Item Type");
                cbItemType.Focus();
                return;
            }
            if (cbCategory.SelectedIndex <= 0)
            {
                CommonFunc.DisplayErrorMessage("Select Category");
                cbCategory.Focus();
                return;
            }
            if (cbSubCategory.SelectedIndex <= 0)
            {
                CommonFunc.DisplayErrorMessage("Select Sub-Category");
                cbSubCategory.Focus();
                return;
            }

            if (!rbActive.Checked && !rbInactive.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Active / Inactive");
                return;
            }
            //if (String.IsNullOrEmpty(txtWholeSale.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Enter Whole Sale Price");
            //    txtWholeSale.Focus();
            //    return;
            //}
            //if (String.IsNullOrEmpty(txtSemi.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Enter Semi-Whole Sale Price");
            //    txtSemi.Focus();
            //    return;
            //}
            //if (String.IsNullOrEmpty(txtRetail.Text.Trim()))
            //{
            //    CommonFunc.DisplayErrorMessage("Enter Retail Price");
            //    txtRetail.Focus();
            //    return;
            //}


            if (!String.IsNullOrEmpty(txtWholeSale.Text.Trim()))
            {
                if (Convert.ToDecimal(txtWholeSale.Text) < 0)
                {
                    CommonFunc.DisplayErrorMessage("Invalid rate for Wholesale");
                    txtWholeSale.Focus();
                    return;
                }
            }

            if (!String.IsNullOrEmpty(txtSemi.Text.Trim()))
            {
                if (Convert.ToDecimal(txtSemi.Text) < 0)
                {
                    CommonFunc.DisplayErrorMessage("Invalid rate for Semi-Wholesale");
                    txtSemi.Focus();
                    return;
                }
            }

            if (!String.IsNullOrEmpty(txtRetail.Text.Trim()))
            {
                if (Convert.ToDecimal(txtRetail.Text) < 0)
                {
                    CommonFunc.DisplayErrorMessage("Invalid rate for Retail");
                    txtRetail.Focus();
                    return;
                }
            }

            if (cbConversion.SelectedIndex <= 0)
            {
                CommonFunc.DisplayErrorMessage("Select Conversion");
                cbConversion.Focus();
                return;
            }
            if (cbHSN.SelectedIndex <= 0)
            {
                CommonFunc.DisplayErrorMessage("Select HSN Code");
                cbHSN.Focus();
                return;
            }

            ItemsEntity itemsEntity = new ItemsEntity();
            itemsEntity.ItemCode = txtItemCode.Text;
            itemsEntity.ItemName = txtItemName.Text;
            itemsEntity.CompanyId = Convert.ToInt32(cbCompany.SelectedValue);
            itemsEntity.CategoryId = Convert.ToInt32(cbCategory.SelectedValue);
            itemsEntity.SubCategoryId = Convert.ToInt32(cbSubCategory.SelectedValue);
            itemsEntity.ItemType = Convert.ToInt32(cbItemType.SelectedValue);
            itemsEntity.WholeSalePrice = String.IsNullOrEmpty(txtWholeSale.Text.Trim()) ? 0 : Convert.ToDecimal(txtWholeSale.Text);
            itemsEntity.SemiWholeSalePrice = String.IsNullOrEmpty(txtSemi.Text.Trim()) ? 0 : Convert.ToDecimal(txtSemi.Text);
            itemsEntity.RetailPrice = String.IsNullOrEmpty(txtRetail.Text.Trim()) ? 0 : Convert.ToDecimal(txtRetail.Text);
            itemsEntity.ConversionId = Convert.ToInt32(cbConversion.SelectedValue);
            itemsEntity.HSNId = Convert.ToInt32(cbHSN.SelectedValue);
            if (String.IsNullOrEmpty(lblItemId.Text.Trim()))
            {
                itemsEntity.DMLType = 'I';
            }
            else
            {
                itemsEntity.ItemId = Convert.ToInt32(lblItemId.Text);
                itemsEntity.DMLType = 'U';
            }

            itemsEntity.quantities = new List<ItemConversionQuantity>();
            ItemConversionQuantity quantity = null;
            foreach (DataGridViewRow viewRow in dgvConversions.Rows)
            {
                if (viewRow.Cells[2].Value == null || String.IsNullOrEmpty(viewRow.Cells[2].Value.ToString()))
                {
                    CommonFunc.DisplayErrorMessage("Enter Qunatity");
                    viewRow.Cells[2].Selected = true;
                    return;
                }
                if (Convert.ToInt32(viewRow.Cells[2].Value) == 0)
                {
                    CommonFunc.DisplayErrorMessage("Qunatity cannot be Zero");
                    viewRow.Cells[2].Selected = true;
                    return;
                }
                quantity = new ItemConversionQuantity
                {
                    ItemTypeId = Convert.ToInt32(viewRow.Cells[0].Value),
                    Quantity = Convert.ToInt32(viewRow.Cells[2].Value),
                    WholeSale = viewRow.Cells[3].Value == null || String.IsNullOrEmpty(viewRow.Cells[3].Value.ToString().Trim()) ? 0 : Convert.ToDecimal(viewRow.Cells[3].Value),
                    SemiWholeSale = viewRow.Cells[4].Value == null || String.IsNullOrEmpty(viewRow.Cells[4].Value.ToString().Trim()) ? 0 : Convert.ToDecimal(viewRow.Cells[4].Value),
                    Retail = viewRow.Cells[5].Value == null || String.IsNullOrEmpty(viewRow.Cells[5].Value.ToString().Trim()) ? 0 : Convert.ToDecimal(viewRow.Cells[5].Value)
                };

                itemsEntity.quantities.Add(quantity);
            }

            if (itemsEntity.quantities.Where(x => x.ItemTypeId == itemsEntity.ItemType).Count() <= 0)
            {
                CommonFunc.DisplayErrorMessage("Mismatch in item type selection between Item type and Conversion");
                return;
            }

            //if (itemsEntity.quantities.Where(x => x.Quantity == 1).Count() > 1)
            //{
            //    CommonFunc.DisplayErrorMessage("One or more item types has quantity is 1");
            //    return;
            //}

            int Quan = 1;
            for (int i = itemsEntity.quantities.Count - 1; i >= 0; i--)
            {
                itemsEntity.quantities[i].QuantityPerType = Quan;
                Quan = Quan * itemsEntity.quantities[i].Quantity;
            }
            itemsEntity.IsActive = rbActive.Checked;

            CommonDal common = new CommonDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            common.SaveItems(itemsEntity, alertMsgInfo);

            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.SaveMessage(alertMsgInfo.Message);
                if (isEdit)
                {
                    if (fromForm)
                    {
                        ViewInventory view = new ViewInventory();
                        lblItemId.Text = string.Empty;
                        view.MdiParent = this.MdiParent;
                        this.Close();
                        view.Show();
                    }
                    else
                    {
                        InactiveItems view = new InactiveItems();
                        view.MdiParent = this.MdiParent;
                        this.Close();
                        view.Show();
                    }
                }
                else
                    clearControls();
            }
            else
            {
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearControls();
        }

        private void clearControls()
        {
            txtItemCode.Text = string.Empty;
            txtItemName.Text = string.Empty;
            cbCompany.SelectedIndex = 0;
            cbItemType.SelectedIndex = 0;
            cbCategory.SelectedIndex = 0;
            cbSubCategory.SelectedIndex = 0;
            txtWholeSale.Text = string.Empty;
            txtSemi.Text = string.Empty;
            txtRetail.Text = string.Empty;
            cbConversion.SelectedIndex = 0;
            if (dgvConversions.Rows.Count > 0)
                dgvConversions.Rows.Clear();
            dgvConversions.Refresh();
            lblSemi.Text = string.Empty;
            lblRetail.Text = string.Empty;
            //gbItem.Visible = false;
            lblItemId.Text = string.Empty;
            txtItemCode.Focus();
        }

        private void txtWholeSale_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtWholeSale);
        }

        private void TxtSemi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtSemi);
        }

        private void txtRetail_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = CommonFunc.IsDecimal(e, txtRetail);
        }

        private void txtWholeSale_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtWholeSale.Text.Trim()))
            {
                if (Convert.ToDecimal(txtWholeSale.Text) < 0)
                {
                    CommonFunc.DisplayErrorMessage("Invalid rate for Wholesale");
                    txtWholeSale.Focus();
                    return;
                }

                int quantity = 1;
                for (int i = dgvConversions.Rows.Count - 1; i > -1; i--)
                {
                    if (dgvConversions.Rows[i].Cells[2].Value != null
                        && dgvConversions.Rows[i].Cells[2].Value.ToString() != "")
                    {
                        quantity *= Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value);
                        dgvConversions.Rows[i].Cells[3].Value = ((quantity * Convert.ToDecimal(txtWholeSale.Text)) /
                            Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value)).ToString();
                    }
                    else
                        dgvConversions.Rows[i].Cells[3].Value = "";
                }

                if (!String.IsNullOrEmpty(lblSemi.Text))
                {
                    txtSemi.Text = Math.Round(Convert.ToDecimal(txtWholeSale.Text) + (Convert.ToDecimal(txtWholeSale.Text) * Convert.ToDecimal(lblSemi.Text) / 100), 0).ToString();
                    txtRetail.Text = Math.Round(Convert.ToDecimal(txtWholeSale.Text) + (Convert.ToDecimal(txtWholeSale.Text) * Convert.ToDecimal(lblRetail.Text) / 100), 0).ToString();
                }
            }
            else
            {
                for (int i = dgvConversions.Rows.Count - 1; i > -1; i--)
                {
                    dgvConversions.Rows[i].Cells[3].Value = "";
                }
            }
        }

        private void txtSemi_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSemi.Text.Trim()))
            {
                if (Convert.ToDecimal(txtSemi.Text) < 0)
                {
                    CommonFunc.DisplayErrorMessage("Invalid rate for Semi-Wholesale");
                    txtSemi.Focus();
                    return;
                }

                int quantity = 1;
                for (int i = dgvConversions.Rows.Count - 1; i > -1; i--)
                {
                    if (dgvConversions.Rows[i].Cells[2].Value != null
                        && dgvConversions.Rows[i].Cells[2].Value.ToString() != "")
                    {
                        quantity *= Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value);
                        dgvConversions.Rows[i].Cells[4].Value = (quantity * Convert.ToDecimal(txtSemi.Text) /
                            Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value)).ToString();
                    }
                    else
                        dgvConversions.Rows[i].Cells[4].Value = "";
                }
            }
            else
            {
                for (int i = dgvConversions.Rows.Count - 1; i > -1; i--)
                {
                    dgvConversions.Rows[i].Cells[4].Value = "";
                }
            }
        }

        private void txtRetail_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRetail.Text.Trim()))
            {
                if (Convert.ToDecimal(txtRetail.Text) < 0)
                {
                    CommonFunc.DisplayErrorMessage("Invalid rate for Retail");
                    txtRetail.Focus();
                    return;
                }

                int quantity = 1;
                for (int i = dgvConversions.Rows.Count - 1; i > -1; i--)
                {
                    if (dgvConversions.Rows[i].Cells[2].Value != null
                        && dgvConversions.Rows[i].Cells[2].Value.ToString() != "")
                    {
                        quantity *= Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value);
                        dgvConversions.Rows[i].Cells[5].Value = ((quantity * Convert.ToDecimal(txtRetail.Text)) /
                            Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value)).ToString();
                    }
                    else
                        dgvConversions.Rows[i].Cells[5].Value = "";
                }
            }
            else
            {
                for (int i = dgvConversions.Rows.Count - 1; i > -1; i--)
                {
                    dgvConversions.Rows[i].Cells[5].Value = "";
                }
            }
        }
        Control cntObject;

        private void dgvConversions_EditingControlShowing(object sender,
            DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.TextChanged += new EventHandler(textBox_TextChanged);
            cntObject = e.Control;
            cntObject.TextChanged += textBox_TextChanged;
        }

        // TextBox TextChanged Event
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (cntObject.Text != string.Empty)
            {
                //textBox.Text = cntObject.Text;
            }
        }

        private void dgvConversions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (string.IsNullOrEmpty(txtWholeSale.Text.Trim()))
            {
                txtWholeSale.Text = "0.00";
            }
            if (string.IsNullOrEmpty(txtSemi.Text.Trim()))
            {
                txtSemi.Text = "0.00";
            }
            if (string.IsNullOrEmpty(txtRetail.Text.Trim()))
            {
                txtRetail.Text = "0.00";
            }
            int quantity = 1;
            for (int i = dgvConversions.Rows.Count - 1; i > -1; i--)
            {
                if (dgvConversions.Rows[i].Cells[2].Value != null
                    && dgvConversions.Rows[i].Cells[2].Value.ToString() != "")
                {
                    quantity *= Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value);
                    dgvConversions.Rows[i].Cells[3].Value = ((quantity * Convert.ToDecimal(txtWholeSale.Text)) /
                        Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value)).ToString();
                    dgvConversions.Rows[i].Cells[4].Value = ((quantity * Convert.ToDecimal(txtSemi.Text)) /
                        Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value)).ToString();
                    dgvConversions.Rows[i].Cells[5].Value = ((quantity * Convert.ToDecimal(txtRetail.Text)) /
                        Convert.ToInt32(dgvConversions.Rows[i].Cells[2].Value)).ToString();
                }
                else
                {
                    dgvConversions.Rows[i].Cells[3].Value = "";
                    dgvConversions.Rows[i].Cells[4].Value = "";
                    dgvConversions.Rows[i].Cells[5].Value = "";
                }
            }

        }

        private void cbCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCompany.SelectedIndex > 0)
            {
                CommonDal common = new CommonDal();
                CompanyEntity entity = common.GetCompanyMargin(Convert.ToInt32(cbCompany.SelectedValue));
                lblSemi.Text = entity.SemiWholeSale.ToString();
                lblRetail.Text = entity.Retail.ToString();
            }
            else
            {
                lblSemi.Text = string.Empty;
                lblRetail.Text = string.Empty;
            }
        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonFunc.SubCategoryFill(cbSubCategory, cbCategory.SelectedValue as int?, false);
        }
    }
}
