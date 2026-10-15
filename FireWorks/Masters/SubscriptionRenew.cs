using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Data;
using System.Windows.Forms;

namespace FireWorks.Masters
{
    public partial class SubscriptionRenew : Form
    {
        // private readonly ICommon dal = new CommonDal();
        DataView View = null;

        public SubscriptionRenew()
        {
            // dal = new CommonDal();
            InitializeComponent();
            this.Text = CommonFunc.ApplicationTitile;
            // show loading overlay while form initializes
            FireWorks.UI.UiHelpers.ShowLoading(this, "Loading items...");
            //try
            //{
            //    // apply styling helpers at runtime for this form
            //    FireWorks.UI.UiHelpers.ApplyFormStyle(this);
            //}
            //catch { }
            // Restrict range
            DateTime expiryDate = SubscriptionManager.GetExpiryDate();

            // Step 1: Set min/max range
            dtpExpiry.MinDate = DateTime.Now;              // today is minimum
            dtpExpiry.MaxDate = DateTime.Now.AddYears(5);  // 5 years ahead

            // Step 2: Adjust expiry date if out of range
            if (expiryDate == DateTime.MaxValue)
            {
                // Permanent → disable picker
                dtpExpiry.Enabled = false;
            }
            else if (expiryDate < dtpExpiry.MinDate)
            {
                // Expired → reset to MinDate
                dtpExpiry.Value = dtpExpiry.MinDate;
            }
            else if (expiryDate > dtpExpiry.MaxDate)
            {
                // Too far in future → clamp to MaxDate
                dtpExpiry.Value = dtpExpiry.MaxDate;
            }
            else
            {
                // Valid → bind normally
                dtpExpiry.Value = expiryDate;
            }
            // hide loading overlay once initialization completes
            FireWorks.UI.UiHelpers.HideLoading(this);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            UserDal user = new UserDal();
            AlertMsgInfo alertMsg = new AlertMsgInfo();
            if (dtpExpiry.Checked)
            {
                SubscriptionManager.Renew(dtpExpiry.Value);
                user.AllowUser(0, dtpExpiry.Value, alertMsg);
                CommonFunc.SaveMessage("Subscription renewed until: " + dtpExpiry.Value.ToShortDateString());
            }
            else
            {
                SubscriptionManager.ActivatePermanent();
                dtpExpiry.Enabled = false;
                user.AllowUser(1, dtpExpiry.Value, alertMsg);
                CommonFunc.SaveMessage("Application permanently activated!");
            }
            CommonFunc.ResetAllControls(this);
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            CommonFunc.ResetAllControls(this);
            dtpExpiry.Checked = false;
        }

    }
}
