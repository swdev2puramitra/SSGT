using CrystalDecisions.Shared;
using FireWorks.Reports;
using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class SearchBills : Form
    {
        bool formload = false;
        string selectedPath = null;
        public SearchBills()
        {
            InitializeComponent();
            dtpBill.MaxDate = DateTime.Today;
            dtpBill.Value = DateTime.Today;
            dtpBill.Checked = true;
            WindowState = FormWindowState.Maximized;
            //Height = 550;
            dgvBills.Height = 500;
            CommonFunc.fillCustomer(cbCustomer);
            fillGrid();
        }
        private void fillGridUpdate()
        {
            fillGrid();
        }
        private void fillGrid()
        {
            Int64? PhoneNo = null, BillId = null;
            DateTime? CreateDate = null;
            int? CustomerType = null;
            char? IsEstimate = null;
            string customer = null;

            if (!String.IsNullOrEmpty(txtPhone.Text.Trim()))
                PhoneNo = Convert.ToInt64(txtPhone.Text);
            if (!String.IsNullOrEmpty(txtCustomer.Text.Trim()))
                customer = txtCustomer.Text.Trim();
            if (!String.IsNullOrEmpty(txtBillId.Text.Trim()))
                BillId = Convert.ToInt64(txtBillId.Text);
            if (cbCustomer.SelectedValue != null)
                if (!String.IsNullOrEmpty(cbCustomer.SelectedValue.ToString()))
                    CustomerType = Convert.ToInt32(cbCustomer.SelectedValue);
            if (dtpBill.Checked)
                CreateDate = Convert.ToDateTime(dtpBill.Value);
            if (rbEstimate.Checked || rbInvoice.Checked)
                IsEstimate = rbEstimate.Checked ? 'E' : 'I';

            if (!formload)
            {
                CreateDate = DateTime.Now;
                formload = true;
            }
            BillDal dal = new BillDal();
            dgvBills.DataSource = dal.GetSavedBills(PhoneNo, customer, CreateDate, BillId, CustomerType, IsEstimate);
            DisplayGrid(dgvBills);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, false, false);
            objGrid.AutoGenerateColumns = false;
            objGrid.Columns["SlNo"].HeaderText = "Sl. No.";
            objGrid.Columns["SlNo"].Width = 40;
            objGrid.Columns["SlNo"].DisplayIndex = 0;
            objGrid.Columns["SlNo"].ReadOnly = true;

            objGrid.Columns["CustomerName"].HeaderText = "Customer Name";
            objGrid.Columns["CustomerName"].Width = 180;
            objGrid.Columns["CustomerName"].DisplayIndex = 1;
            objGrid.Columns["CustomerName"].ReadOnly = true;

            objGrid.Columns["CustAddress"].HeaderText = "Address";
            objGrid.Columns["CustAddress"].Width = 100;
            objGrid.Columns["CustAddress"].DisplayIndex = 2;
            objGrid.Columns["CustAddress"].ReadOnly = true;

            objGrid.Columns["CustomerType"].HeaderText = "Customer Type";
            objGrid.Columns["CustomerType"].Width = 70;
            objGrid.Columns["CustomerType"].DisplayIndex = 3;
            objGrid.Columns["CustomerType"].ReadOnly = true;

            objGrid.Columns["BillId"].HeaderText = "Bill Id";
            objGrid.Columns["BillId"].Width = 80;
            objGrid.Columns["BillId"].DisplayIndex = 4;
            objGrid.Columns["BillId"].ReadOnly = true;
            objGrid.Columns["BillId"].Visible = false;

            objGrid.Columns["BillNo"].HeaderText = "Bill No";
            objGrid.Columns["BillNo"].Width = 80;
            objGrid.Columns["BillNo"].DisplayIndex = 5;
            objGrid.Columns["BillNo"].ReadOnly = true;

            objGrid.Columns["InvoiceNo"].HeaderText = "Invoice No";
            objGrid.Columns["InvoiceNo"].Width = 60;
            objGrid.Columns["InvoiceNo"].DisplayIndex = 6;
            objGrid.Columns["InvoiceNo"].ReadOnly = true;

            objGrid.Columns["BillAmount"].HeaderText = "Bill Amount";
            objGrid.Columns["BillAmount"].Width = 80;
            objGrid.Columns["BillAmount"].DisplayIndex = 7;
            objGrid.Columns["BillAmount"].ReadOnly = true;

            objGrid.Columns["BillDate"].HeaderText = "Bill Date";
            objGrid.Columns["BillDate"].Width = 90;
            objGrid.Columns["BillDate"].DisplayIndex = 8;
            objGrid.Columns["BillDate"].ReadOnly = true;

            objGrid.Columns["PhoneNo"].HeaderText = "Phone No.";
            objGrid.Columns["PhoneNo"].Width = 85;
            objGrid.Columns["PhoneNo"].DisplayIndex = 9;
            objGrid.Columns["PhoneNo"].ReadOnly = true;

            objGrid.Columns["Estimate"].HeaderText = "Estimate?";
            objGrid.Columns["Estimate"].Width = 70;
            objGrid.Columns["Estimate"].DisplayIndex = 10;
            objGrid.Columns["Estimate"].ReadOnly = true;

            objGrid.Columns["WayBill"].HeaderText = "Way Bill";
            objGrid.Columns["WayBill"].Width = 70;
            objGrid.Columns["WayBill"].DisplayIndex = 11;
            objGrid.Columns["WayBill"].ReadOnly = true;

            objGrid.Columns["Edit"].HeaderText = "Edit";
            objGrid.Columns["Edit"].Width = 70;
            objGrid.Columns["Edit"].DisplayIndex = 12;
            objGrid.Columns["Edit"].ReadOnly = true;

            //objGrid.Columns["Copy"].HeaderText = "Copy";
            //objGrid.Columns["Copy"].Width = 70;
            //objGrid.Columns["Copy"].DisplayIndex = 13;
            //objGrid.Columns["Copy"].ReadOnly = true;

            //objGrid.Columns["Print"].HeaderText = "Print";
            //objGrid.Columns["Print"].Width = 70;
            //objGrid.Columns["Print"].DisplayIndex = 13;
            //objGrid.Columns["Print"].ReadOnly = true;

            objGrid.Columns["Update"].HeaderText = "Way Bill";
            objGrid.Columns["Update"].Width = 70;
            objGrid.Columns["Update"].DisplayIndex = 13;
            objGrid.Columns["Update"].ReadOnly = true;

            objGrid.Columns["Print"].HeaderText = "Print";
            objGrid.Columns["Print"].Width = 70;
            objGrid.Columns["Print"].DisplayIndex = 14;
            objGrid.Columns["Print"].ReadOnly = true;

            //objGrid.Columns["IsVerified"].HeaderText = "IsVerified";
            //objGrid.Columns["IsVerified"].Width = 70;
            //objGrid.Columns["IsVerified"].DisplayIndex = 16;
            //objGrid.Columns["IsVerified"].ReadOnly = true;

            objGrid.Columns["JSON"].HeaderText = "JSON";
            objGrid.Columns["JSON"].Width = 70;
            objGrid.Columns["JSON"].DisplayIndex = 15;
            objGrid.Columns["JSON"].ReadOnly = true;

            objGrid.Columns["Dispatch"].HeaderText = "Dispatch";
            objGrid.Columns["Dispatch"].Width = 70;
            objGrid.Columns["Dispatch"].DisplayIndex = 16;
            objGrid.Columns["Dispatch"].ReadOnly = true;

            objGrid.Columns["IsInState"].HeaderText = "IsInState";
            objGrid.Columns["IsInState"].Width = 70;
            objGrid.Columns["IsInState"].DisplayIndex = 17;
            objGrid.Columns["IsInState"].ReadOnly = true;

            objGrid.Columns["InvoiceStatus"].HeaderText = "InvoiceStatus";
            objGrid.Columns["InvoiceStatus"].Width = 70;
            objGrid.Columns["InvoiceStatus"].DisplayIndex = 18;
            objGrid.Columns["InvoiceStatus"].ReadOnly = true;
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            if (txtPhone.Text.Length == 10)
                fillGrid();
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtPhone.Text))
                e.Handled = true;
        }

        private void txtBillId_TextChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        private void txtBillId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtBillId.Text))
                e.Handled = true;
        }

        private void dtpBill_ValueChanged(object sender, EventArgs e)
        {
            dtpBill.Format = DateTimePickerFormat.Custom;
            dtpBill.CustomFormat = "dd-MMM-yyyy";
            fillGrid();
        }

        private void dtpBill_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back) || (e.KeyCode == Keys.Delete))
            {
                dtpBill.CustomFormat = " ";
                fillGrid();
            }
        }

        private void dgvBills_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn column &&
                e.RowIndex >= 0)
            {
                if (dgvBills.Rows[e.RowIndex].Cells["InvoiceStatus"].Value.ToString() == "1"
                    && column.Text.Trim().ToLower() != "print")
                {
                    return;
                }

                bool isCopy = false;
                Int64 BillId = Convert.ToInt64(dgvBills.Rows[e.RowIndex].Cells["BillId"].Value.ToString().Replace("E", ""));
                switch (column.Text.Trim().ToLower())
                {
                    case "print":
                        //IsInState,CustomerType,Estimate
                        Billing bill = new Billing();
                        if (dgvBills.Rows[e.RowIndex].Cells["CustomerType"].Value.ToString().ToLower() == "Alpha".ToLower()
                            || dgvBills.Rows[e.RowIndex].Cells["CustomerType"].Value.ToString().ToLower() == "Beta".ToLower())
                        {
                            if (dgvBills.Rows[e.RowIndex].Cells["Estimate"].Value.ToString().ToLower() == "Yes".ToLower())
                            {
                                bill.PrintEstimate(BillId, false);
                            }
                            else
                            {
                                if (Convert.ToBoolean(dgvBills.Rows[e.RowIndex].Cells["IsInState"].Value))
                                {
                                    bill.PrintInvoice(BillId);
                                }
                                else
                                {
                                    bill.PrintInvoiceNational(BillId);
                                }
                            }
                        }
                        else
                        {
                            bill.PrintRetail(BillId);
                        }
                        /*
                        Reports.rptPacking estimate = new Reports.rptPacking();
                        BillDal billDal123 = new BillDal();
                        DataSet dataSet = billDal123.GetPackingDetailsById(BillId);
                        estimate.Database.Tables["USP_GetBillGodownWiseAllocation"].SetDataSource(dataSet.Tables[0]);
                        estimate.Database.Tables["USP_GET_ShotDetails"].SetDataSource(ShopDetails.dtShopDetails);

                        estimate.PrintOptions.PaperOrientation = PaperOrientation.Portrait;
                        estimate.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
                        //estimate.PrintToPrinter(1, false, 0, 0);
                        using (ViewReport report = new ViewReport(estimate))
                        {
                            report.Text = "Allocation Report";
                            report.ShowDialog();
                        }
                        */
                        return;
                    case "verify":
                        if (Convert.ToBoolean(dgvBills.Rows[e.RowIndex].Cells["IsVerified1"].Value))
                        {
                            CommonFunc.DisplayErrorMessage("Bill is already verified");
                            return;
                        }
                        if (MessageBox.Show(this, "Is the item particulars are verified?", CommonFunc.ApplicationTitile,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            BillDal billDal = new BillDal();
                            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
                            billDal.VeifyBill(BillId, alertMsgInfo);
                            if (alertMsgInfo.SaveStatus)
                            {
                                CommonFunc.SaveMessage(alertMsgInfo.Message);
                                fillGrid();
                            }
                            else
                                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
                        }
                        return;
                    case "copy":
                        isCopy = true;
                        break;
                    case "update":
                        if (dgvBills.Rows[e.RowIndex].Cells["InvoiceNo"].Value.ToString() == "")
                        {
                            CommonFunc.DisplayErrorMessage("Way Bill No. is allowed to Inovoice bills only.");
                            return;
                        }
                        WayBill way = new WayBill(BillId, dgvBills.Rows[e.RowIndex].Cells["BillNo"].Value.ToString(),
                            dgvBills.Rows[e.RowIndex].Cells["CustomerName"].Value.ToString(), dgvBills.Rows[e.RowIndex].Cells["WayBill"].Value.ToString());
                        way.RefreshDgv += new WayBill.DoEvent(fillGridUpdate);
                        way.Show();
                        return;
                    case "get":
                        if (dgvBills.Rows[e.RowIndex].Cells["InvoiceNo"].Value.ToString() == "")
                        {
                            CommonFunc.DisplayErrorMessage("JSON is allowed to Inovoice bills only.");
                            return;
                        }
                        BillDal billDal1 = new BillDal();
                        Bill bills = billDal1.GetBillDetailsById(BillId);

                        if (String.IsNullOrEmpty(bills.CustGSTIN))
                        {
                            CommonFunc.DisplayErrorMessage("Customer GSTIN not available");
                            return;
                        }
                        TimeSpan time = DateTime.Now - bills.CreatedDate;
                        if (time.Days > 30)
                        {
                            string key = ConfigurationManager.AppSettings["InvoiceDt"].ToString();
                            bills.CreatedDate = bills.CreatedDate.AddDays(time.Days).AddDays(Convert.ToInt16(key));
                        }
                        List<TaxInvoiceDtls> taxes = new List<TaxInvoiceDtls>();
                        TaxInvoiceDtls taxInvoiceDtls = new TaxInvoiceDtls()
                        {
                            DocDtls = new DocumentDetails()
                            {
                                No = bills.InvoiceNo,
                                Dt = bills.CreatedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                            },
                            BuyerDtls = new BuyerSeller()
                            {
                                Gstin = bills.CustGSTIN,
                                LglNm = bills.Name,
                                Addr1 = bills.Address,
                                Ph = bills.PhoneNo.ToString() == "0" ? null : bills.PhoneNo.ToString(),
                                Loc = bills.Location,
                                Pin = Convert.ToInt32(bills.Pincode),
                                Pos = bills.POS,
                                Stcd = bills.StateCode,
                            },
                            ValDtls = new AmountDetails()
                            {
                                AssVal = Math.Round(bills.BillAmount - (Convert.ToDecimal(bills.DiscountPercentage) * bills.BillAmount / 100), 2),
                                //IgstVal = bills.BillAmount,
                            },
                            ItemList = bills.billedItems.Select(x => new Items()
                            {
                                SlNo = x.SlNo.ToString(),
                                PrdDesc = x.ItemName,
                                IsServc = "N",
                                HsnCd = x.HSNCode,
                                Qty = x.Quantity,
                                Unit = x.UnitName,
                                UnitPrice = Convert.ToDecimal(x.IndividualCost),
                                TotAmt = Math.Round(x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100), 2),
                                AssAmt = Math.Round(x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100), 2),
                                GstRt = Convert.ToDecimal(x.IGST),
                                IgstAmt = Convert.ToBoolean(bills.IsInState) ? 0 : Math.Round(Convert.ToDecimal(x.IGST) * (x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100)) / 100, 2),
                                CgstAmt = Convert.ToBoolean(bills.IsInState) ? Math.Round(Convert.ToDecimal(x.CGST) * (x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100)) / 100, 2) : 0,
                                SgstAmt = Convert.ToBoolean(bills.IsInState) ? Math.Round(Convert.ToDecimal(x.SGST) * (x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100)) / 100, 2) : 0,
                                TotItemVal = Math.Round(x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100), 2)
                                + (Convert.ToBoolean(bills.IsInState) ? 0 : Math.Round(Convert.ToDecimal(x.IGST) * (x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100)) / 100, 2))
                                + (Convert.ToBoolean(bills.IsInState) ? Math.Round(Convert.ToDecimal(x.SGST) * (x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100)) / 100, 2) : 0)
                                + (Convert.ToBoolean(bills.IsInState) ? Math.Round(Convert.ToDecimal(x.CGST) * (x.Quantity * Convert.ToDecimal(x.IndividualCost) - (x.Quantity * Convert.ToDecimal(x.IndividualCost) * Convert.ToDecimal(bills.DiscountPercentage) / 100)) / 100, 2) : 0)
                            }).ToList()
                        };
                        taxInvoiceDtls.ValDtls.IgstVal = taxInvoiceDtls.ItemList.Sum(x => x.IgstAmt);
                        taxInvoiceDtls.ValDtls.SgstVal = taxInvoiceDtls.ItemList.Sum(x => x.SgstAmt);
                        taxInvoiceDtls.ValDtls.CgstVal = taxInvoiceDtls.ItemList.Sum(x => x.CgstAmt);
                        taxInvoiceDtls.ValDtls.TotInvVal = taxInvoiceDtls.ItemList.Sum(x => x.TotItemVal);

                        taxes.Add(taxInvoiceDtls);

                        string json = JsonConvert.SerializeObject(taxes);
                        string folderPath = "";
                        FolderBrowserDialog directchoosedlg = new FolderBrowserDialog();
                        if (selectedPath != null)
                            directchoosedlg.SelectedPath = selectedPath;
                        else
                            directchoosedlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        if (directchoosedlg.ShowDialog() == DialogResult.OK)
                        {
                            folderPath = directchoosedlg.SelectedPath + "\\";
                            selectedPath = directchoosedlg.SelectedPath + "\\";
                            AlertMsgInfo alertMsg = new AlertMsgInfo();
                            billDal1.UpdateJSONStatus(BillId, alertMsg);
                            if (alertMsg.SaveStatus)
                            {
                                File.WriteAllText(folderPath + bills.InvoiceNo + ".json", json);
                                CommonFunc.SaveMessage("JSON file generated successfully");
                            }
                            else
                                CommonFunc.DisplayErrorMessage(alertMsg.Message);
                        }
                        else
                        {
                            CommonFunc.DisplayErrorMessage("Generation of JSON is cancelled");
                            return;
                        }
                        return;
                    case "dispatch":
                        if (UserSessionEntity.RoleId == 1)
                        {
                            if (dgvBills.Rows[e.RowIndex].Cells["Estimate"].Value.ToString() == "Yes")
                            {
                                Dispatch dispatch = new Dispatch(BillId,
                                    dgvBills.Rows[e.RowIndex].Cells["BillNo"].Value.ToString(),
                                    dgvBills.Rows[e.RowIndex].Cells["CustomerName"].Value.ToString(),
                                    Convert.ToDecimal(dgvBills.Rows[e.RowIndex].Cells["BillAmount"].Value));
                                dispatch.RefreshDgv += new Dispatch.DoEvent(fillGridUpdate);
                                dispatch.Show();
                            }
                            else
                            {
                                CommonFunc.DisplayErrorMessage("Dispatch is allowed to Estimate bills only.");
                            }
                        }
                        else
                        {
                            CommonFunc.DisplayErrorMessage("Dispatch is allowed to Admin only.");
                        }
                        return;
                    default:
                        break;
                }

                //TODO - Button Clicked - Execute Code Here
                Billing billing = new Billing(BillId, isCopy)
                {
                    MdiParent = this.MdiParent
                };
                this.Close();
                billing.Show();
            }
        }

        private void rbEstimate_CheckedChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        private void cbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillGrid();
        }

        private void dgvBills_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void dgvBills_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            string[] ButtonColumns = { "Edit", "Copy", "Update", "IsVerified", "JSON" };
            foreach (string col in ButtonColumns)
            {
                if (dgvBills.Columns[e.ColumnIndex].Name == col && e.RowIndex >= 0)
                {
                    var status = dgvBills.Rows[e.RowIndex].Cells["InvoiceStatus"].Value?.ToString();

                    if (status == "1")
                    {
                        e.PaintBackground(e.ClipBounds, true);
                        e.Graphics.FillRectangle(Brushes.LightGray, e.CellBounds);
                        TextRenderer.DrawText(e.Graphics, col, e.CellStyle.Font, e.CellBounds, Color.DarkGray);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}