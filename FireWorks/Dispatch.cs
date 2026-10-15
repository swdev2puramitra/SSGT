using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class Dispatch : Form
    {
        public delegate void DoEvent();
        public event DoEvent RefreshDgv;
        public Dispatch(Int64 BillId, string BillNo, string Name, decimal Amount)
        {
            InitializeComponent();
            lblBillId.Text = BillId.ToString();
            lblBillNo.Text = BillNo;
            lblName.Text = Name;
            lblAmount.Text = Amount.ToString();
        }

        private void btnDispatch_Click(object sender, EventArgs e)
        {
            BillDal dal = new BillDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            dal.UpdateDispatch(Convert.ToInt64(lblBillId.Text), 'D', alertMsgInfo);
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
    }
}
