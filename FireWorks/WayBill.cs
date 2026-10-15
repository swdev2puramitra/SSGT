using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class WayBill : Form
    {
        public delegate void DoEvent();
        public event DoEvent RefreshDgv;
        public WayBill(Int64 BillId, string BillNo, string Name, string WayBill)
        {
            InitializeComponent();
            lblBillId.Text = BillId.ToString();
            lblBillNo.Text = BillNo;
            lblName.Text = Name;
            txtWayBill.Text = WayBill;
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtWayBill.Text))
            {
                CommonFunc.DisplayErrorMessage("Way Bill can not be blank");
                txtWayBill.Focus();
                return;
            }
            BillDal dal = new BillDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            dal.UpdateWayBill(Convert.ToInt64(lblBillId.Text), txtWayBill.Text, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                CommonFunc.SaveMessage(alertMsgInfo.Message);
                this.RefreshDgv();
                this.Close();
            }
            else
            {
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
            }
        }

        private void txtWayBill_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CommonFunc.isNumber(e.KeyChar, txtWayBill.Text))
                e.Handled = true;
        }
    }
}
