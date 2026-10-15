using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using FireWorksEntity.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace FireWorks.Masters
{
    public partial class ItemConversion : Form
    {
        public ItemConversion()
        {
            InitializeComponent();
            this.Text = CommonFunc.ApplicationTitile;
            AddFirstRow();
            dgvItems.AllowUserToAddRows = false;
            FillGrid();
            lblConversionId.Text = string.Empty;
        }

        public void AddRow()
        {
            foreach (DataGridViewRow dataGridViewRow in dgvItems.Rows)
            {
                if (dataGridViewRow.Cells[0].Value != null)
                {
                    if (dataGridViewRow.Cells[0].Value != null && String.IsNullOrEmpty(dataGridViewRow.Cells[0].Value.ToString().Trim()))
                    {
                        MessageBox.Show("Enter Sl. No.");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Enter Sl. No.");
                    return;
                }

                if (dataGridViewRow.Cells[1].Value != null)
                {
                    if (dataGridViewRow.Cells[1].Value != null)
                        if (String.IsNullOrEmpty(dataGridViewRow.Cells[1].Value.ToString().Trim()))
                        {
                            MessageBox.Show("Select Item Name");
                            return;
                        }
                }
                else
                {
                    MessageBox.Show("Select Item Name");
                    return;
                }

                if (dataGridViewRow.Cells[2].Value != null)
                {
                    if (dataGridViewRow.Cells[2].Value != null)
                        if (String.IsNullOrEmpty(dataGridViewRow.Cells[2].Value.ToString().Trim()))
                        {
                            MessageBox.Show("Enter Item Order");
                            return;
                        }
                }
                else
                {
                    MessageBox.Show("Enter Item Order");
                    return;
                }
            }

            DataGridViewRow viewRow = (DataGridViewRow)dgvItems.Rows[0].Clone();
            viewRow.Cells[0].Value = (dgvItems.Rows.Count + 1).ToString();
            dgvItems.Rows.Add(viewRow);
        }

        public void AddFirstRow()
        {
            dgvItems.Columns.Clear();
            dgvItems.ColumnCount = 2;
            dgvItems.Columns[0].Name = "Sl. No.";
            dgvItems.Columns[0].DataPropertyName = "SlNo";
            dgvItems.Columns[0].Width = 80;

            DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
            cmb.HeaderText = "Item Type";
            cmb.Name = "ItemTypeName";
            CommonDal common = new CommonDal();
            DataTable table = common.GetItemType(true);
            DataRow dataRow = table.NewRow();
            dataRow["TypeName"] = "--Item Type--";
            table.Rows.InsertAt(dataRow, 0);
            cmb.DataSource = table;
            cmb.DisplayMember = "TypeName";
            cmb.ValueMember = "Id";
            dgvItems.Columns.Insert(1, cmb);

            dgvItems.Columns[2].Name = "Item Order";
            dgvItems.Columns[2].DataPropertyName = "ItemOrder";

            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
            {
                HeaderText = "Delete",
                Name = "Delete",
                Text = "Delete",
                DataPropertyName = "Delete",
                UseColumnTextForButtonValue = true
            };
            dgvItems.Columns.Insert(3, btnDelete);

            dgvItems.Rows.Add(new string[] { "1", null, "" });
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void FillGrid()
        {
            CommonDal common = new CommonDal();
            List<ConversionsEntity> conversions = common.GetConversions(null);
            dgvConvesions.DataSource = (from m in conversions
                                        select new
                                        {
                                            ConversionId = m.ConversionId,
                                            ConversionName = m.ConversionName,
                                            IsActive = m.IsActive ? "Yes" : "No"
                                        }).ToList();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtConvesion.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Conversion Name.");
                return;
            }

            if (!rbActive.Checked && !rbInactive.Checked)
            {
                CommonFunc.DisplayErrorMessage("Select Active / Inactive");
                return;
            }
            ConversionsEntity conversions = new ConversionsEntity();
            conversions.ConversionName = txtConvesion.Text.Trim();
            conversions.IsActive = rbActive.Checked;

            conversions.ItemType = new List<ItemTypeEntity>();
            ItemTypeEntity itemType = null;
            int i = 1;
            foreach (DataGridViewRow dataGridViewRow in dgvItems.Rows)
            {
                itemType = new ItemTypeEntity();
                if (dataGridViewRow.Cells[0].Value != null)
                {
                    if (String.IsNullOrEmpty(dataGridViewRow.Cells[0].Value.ToString().Trim()))
                    {
                        MessageBox.Show("Enter Sl. No.");
                        return;
                    }
                    itemType.SlNo = i++;
                }
                else
                {
                    MessageBox.Show("Enter Sl. No.");
                    return;
                }

                if (dataGridViewRow.Cells[1].Value != null)
                {
                    if (String.IsNullOrEmpty(dataGridViewRow.Cells[1].Value.ToString().Trim()))
                    {
                        MessageBox.Show("Select Item Name");
                        return;
                    }
                    itemType.ItemTypeId = Convert.ToInt32(dataGridViewRow.Cells[1].Value);
                }
                else
                {
                    MessageBox.Show("Select Item Name");
                    return;
                }

                if (dataGridViewRow.Cells[2].Value != null)
                {
                    if (String.IsNullOrEmpty(dataGridViewRow.Cells[2].Value.ToString().Trim()))
                    {
                        MessageBox.Show("Enter Item Order");
                        return;
                    }
                    itemType.ItemOrder = Convert.ToInt32(dataGridViewRow.Cells[2].Value);
                }
                else
                {
                    MessageBox.Show("Enter Item Order");
                    return;
                }
                conversions.ItemType.Add(itemType);
            }

            if (conversions.ItemType.Count == 0)
            {
                MessageBox.Show("Add atleast one Item Type");
                return;
            }

            int items = conversions.ItemType.GroupBy(m => m.ItemTypeId)
                .Select(m => new
                {
                    ItemTypeId = m.Key,
                    Count = m.Count()
                }).Where(x => x.Count > 1).ToList().Count;

            int ItemOrder = conversions.ItemType.GroupBy(m => m.ItemOrder)
               .Select(m => new
               {
                   ItemTypeId = m.Key,
                   Count = m.Count()
               }).Where(x => x.Count > 1).ToList().Count;

            if (items > 0)
            {
                MessageBox.Show("Item type is repeated");
                return;
            }
            if (ItemOrder > 0)
            {
                MessageBox.Show("Item order is repeated");
                return;
            }
            CommonDal common = new CommonDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            if (!String.IsNullOrEmpty(lblConversionId.Text.Trim()))
            {
                conversions.ConversionId = Convert.ToInt32(lblConversionId.Text);
                conversions.DMLType = 'U';
            }
            else
                conversions.DMLType = 'I';
            conversions.IsActive = rbActive.Checked;
            common.SaveConversion(conversions, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                lblConversionId.Text = string.Empty;
                txtConvesion.Text = string.Empty;
                txtConvesion.Focus();
                dgvItems.Rows.Clear();
                dgvItems.Refresh();
                AddFirstRow();
                gbCatId.Visible = false;
                FillGrid();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
            }
            else
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
        }

        private void dgvConvesions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lblConversionId.Text = dgvConvesions.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtConvesion.Text = dgvConvesions.Rows[e.RowIndex].Cells[1].Value.ToString();
                bool status = false;
                if (dgvConvesions.Rows[e.RowIndex].Cells[2].Value.ToString() == "Yes")
                    status = true;
                rbActive.Checked = status;
                rbInactive.Checked = !status;
                gbCatId.Visible = true;

                dgvItems.Columns.Clear();
                dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgvItems.ColumnCount = 2;
                dgvItems.Columns[0].Name = "Sl. No.";
                dgvItems.Columns[0].DataPropertyName = "SlNo";
                dgvItems.Columns[0].Width = 80;
                dgvItems.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                DataGridViewComboBoxColumn cmb = new DataGridViewComboBoxColumn();
                cmb.HeaderText = "Item Type";
                cmb.Name = "ItemTypeName";
                cmb.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                CommonDal common = new CommonDal();
                cmb.DisplayMember = "ItemTypeName";
                cmb.ValueMember = "ItemTypeId";
                cmb.ValueType = typeof(string);
                cmb.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                cmb.DataSource = (from DataRow dr in common.GetItemType(true).AsEnumerable()
                                  select new
                                  {
                                      ItemTypeId = dr["Id"].ToString(),
                                      ItemTypeName = dr["TypeName"].ToString()
                                  }).ToList();
                dgvItems.Columns.Insert(1, cmb);

                dgvItems.Columns[2].Name = "Item Order";
                dgvItems.Columns[2].DataPropertyName = "ItemOrder";

                DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
                {
                    HeaderText = "Delete",
                    Name = "Delete",
                    Text = "Delete",
                    DataPropertyName = "Delete",
                    UseColumnTextForButtonValue = true
                };
                dgvItems.Columns.Insert(3, btnDelete);

                CommonDal dal = new CommonDal();
                List<ItemTypeEntity> itemTypes = dal.GetConversionItems(Convert.ToInt32(lblConversionId.Text));

                foreach (ItemTypeEntity itemType in itemTypes)
                {
                    dgvItems.Rows.Add(new string[] { itemType.SlNo.ToString(), itemType.ItemTypeId.ToString(), itemType.ItemOrder.ToString() });
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblConversionId.Text = string.Empty;
            txtConvesion.Text = string.Empty;
            gbCatId.Visible = false;
            if (dgvItems.Rows.Count > 0)
                dgvItems.Rows.Clear();
            dgvItems.Refresh();
            AddFirstRow();
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                DataGridViewComboBoxCell boxCell = (DataGridViewComboBoxCell)dgvItems.Rows[e.RowIndex].Cells["ItemTypeName"];

                DialogResult result = MessageBox.Show("Are you sure to delete Item Type : "
                    + boxCell.FormattedValue.ToString(),
                    CommonFunc.ApplicationTitile, MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    dgvItems.Rows.Remove(dgvItems.Rows[e.RowIndex]);
                }
            }
        }
    }
}
