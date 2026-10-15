using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class login : BaseForm
    {
        public login()
        {
            InitializeComponent();

            // Opt out this entire form from global auto-styling
            this.Tag = "__no_style";

            PageLoad();
        }

        #region Methods
        private void PageLoad()
        {
            this.Text = CommonFunc.ApplicationTitile;
            try
            {
                // All controls have Tag = "__no_style" in Designer to opt-out of UiAutoStyler
                // All colors are explicitly set in Designer for visibility against dark fireworks background:
                // - Labels: White text on Transparent background
                // - Textboxes: Black text on White background with FixedSingle border
                // - Login button: White text on Blue background
                // - Exit button: Black text on White background
            }
            catch { }
        }

        // Verify Mandatory validations
        private bool IsValidations()
        {
            try
            {
                if (String.IsNullOrEmpty(txtUserName.Text.Trim()))
                {
                    CommonFunc.DisplayErrorMessage("Enter User Name");
                    txtUserName.Focus();
                    return false;
                }

                if (String.IsNullOrEmpty(txtPassword.Text.Trim()))
                {
                    CommonFunc.DisplayErrorMessage("Enter Password");
                    txtPassword.Focus();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, null, "Form : Login", "Method : IsValidations");
                return false;
            }
        }

        // Verify login details
        private bool LoginVerify()
        {
            try
            {
                UserDal user = new UserDal();
                UserEntity Entity = user.UserVerify(txtUserName.Text.Trim());
                if (Entity != null)
                {
                    string key = ConfigurationManager.AppSettings["key"].ToString();
                    //int day = Convert.ToInt16(Hashing.Decrypt(ConfigurationManager.AppSettings["AD"]));
                    //int mon = Convert.ToInt16(Hashing.Decrypt(ConfigurationManager.AppSettings["AM"]));
                    StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                    //if (DateTime.Now > new DateTime(DateTime.Now.Year, mon, day))
                    //{
                    //    CommonFunc.DisplayErrorMessage("Your trail period has been expired.");
                    //    return false;
                    //}

                    if (Entity.ExpiryDate.Date != new DateTime(2017, 12, 2).Date)
                    {
                        if ((Entity.ExpiryDate - DateTime.Now).TotalDays <= 1)
                        {
                            string pwd = txtPassword.Text.Trim();
                            //if (pwd.Substring(pwd.Length - 2) == key)
                            //    if (0 == comparer.Compare(Entity.UserPassword, Hashing.SHA256Generator(pwd.Substring(0, pwd.Length - 2))))
                            //    {
                            //        AlertMsgInfo alertMsg = new AlertMsgInfo();
                            //        user.AllowUser(1, alertMsg);
                            //        txtPassword.Text = string.Empty;
                            //        if (alertMsg.SaveStatus)
                            //        {
                            //            CommonFunc.SaveMessage(alertMsg.Message);
                            //        }
                            //        else
                            //            CommonFunc.DisplayErrorMessage(alertMsg.Message);

                            //        return false;
                            //    }
                            CommonFunc.DisplayErrorMessage("Your trial period has been expired.");
                            return false;
                        }
                    }
                    //pwd = txtPassword.Text.Trim().Substring(0, txtPassword.Text.Trim().Length - 2);
                    if (txtUserName.Text.Trim().ToLower() == "super")
                    {
                        if (0 == comparer.Compare(Entity.UserPassword, Hashing.SHA256Generator(txtPassword.Text.Trim().Substring(0, txtPassword.Text.Trim().Length - 2))))
                        {
                            UserSessionEntity.UserID = Entity.UserID;
                            UserSessionEntity.UserName = Entity.UserName;
                            UserSessionEntity.RoleId = Entity.RoleId;
                            UserSessionEntity.ContactNo = Entity.ContactNo;

                            return true;
                        }
                    }
                    else if (0 == comparer.Compare(Entity.UserPassword, Hashing.SHA256Generator(txtPassword.Text.Trim())))
                    {
                        UserSessionEntity.UserID = Entity.UserID;
                        UserSessionEntity.UserName = Entity.UserName;
                        UserSessionEntity.RoleId = Entity.RoleId;
                        UserSessionEntity.ContactNo = Entity.ContactNo;
                        if (!SubscriptionManager.IsValid())
                        {
                            CommonFunc.DisplayErrorMessage("Your trial period expired. Please renew.");
                            return false;
                        }
                        return true;
                    }
                }

                CommonFunc.DisplayErrorMessage("User Name and Password are Invalid");
                return false;
            }
            catch (Exception ex)
            {
                Errorlog.WriteToErrorLog(ex, null, "Form : Login", "Method : Login Verify");
                CommonFunc.DisplayErrorMessage("User Name and Password are Invalid");
                return false;
            }
        }
        #endregion

        #region Events
        private void login_Load(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void login_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (System.Diagnostics.Process Process in System.Diagnostics.Process.GetProcesses())
            {
                if (Process.ProcessName.Contains("FireWorks"))
                {
                    Process.Kill();
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!IsValidations())
                return;
            bool IsValid = LoginVerify();
            if (IsValid)
            {
                this.Hide();
                MDIFireWorks obj = new MDIFireWorks();
                obj.Show();
            }
        }
        #endregion

    }
}
