using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace FireWorks.Admin
{
    public partial class AddUser : Form
    {
        List<UserEntity> users = null;
        public AddUser()
        {
            InitializeComponent();
            fillGrid();
            CommonFunc.RoleFill(cbRole);
        }

        private void fillGrid()
        {
            UserDal dal = new UserDal();
            users = dal.GetUsers();
            dgvUsers.DataSource = null;
            dgvUsers.DataSource = (from UserEntity user in users
                                   select new
                                   {
                                       user.UserID,
                                       user.UserName,
                                       user.ContactNo,
                                       user.RoleName,
                                       IsActive = user.IsActive ? "Active" : "Inactive"
                                   }).ToArray();
            DisplayGrid(dgvUsers);
        }

        private void DisplayGrid(DataGridView objGrid)
        {
            objGrid.ReadOnly = true;

            CommonFunc.FormatGridLook(objGrid, false, false, false);

            objGrid.Columns["UserID"].HeaderText = "User Id";
            objGrid.Columns["UserID"].Width = 50;
            objGrid.Columns["UserID"].ReadOnly = true;
            objGrid.Columns["UserID"].DisplayIndex = 0;

            objGrid.Columns["UserName"].HeaderText = "User Name";
            objGrid.Columns["UserName"].Width = 100;
            objGrid.Columns["UserName"].ReadOnly = true;
            objGrid.Columns["UserName"].DisplayIndex = 1;

            objGrid.Columns["ContactNo"].HeaderText = "Contact No";
            objGrid.Columns["ContactNo"].Width = 100;
            objGrid.Columns["ContactNo"].ReadOnly = true;
            objGrid.Columns["ContactNo"].DisplayIndex = 2;

            objGrid.Columns["RoleName"].HeaderText = "Role Name";
            objGrid.Columns["RoleName"].Width = 100;
            objGrid.Columns["RoleName"].ReadOnly = true;
            objGrid.Columns["RoleName"].DisplayIndex = 3;

            objGrid.Columns["IsActive"].HeaderText = "Status";
            objGrid.Columns["IsActive"].Width = 100;
            objGrid.Columns["IsActive"].ReadOnly = true;
            objGrid.Columns["IsActive"].DisplayIndex = 4;

            objGrid.Columns["Edit"].HeaderText = "Edit";
            objGrid.Columns["Edit"].Width = 100;
            objGrid.Columns["Edit"].ReadOnly = true;
            objGrid.Columns["Edit"].DisplayIndex = 5;
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                UserEntity entity = users.Where(x => x.UserID == Convert.ToInt32(dgvUsers.Rows[e.RowIndex].Cells["UserId"].Value)).FirstOrDefault();

                txtUserName.Text = entity.UserName;
                txtUserId.Text = entity.UserID.ToString();
                txtContact.Text = entity.ContactNo;
                cbRole.SelectedValue = entity.RoleId;
                rbActive.Checked = entity.IsActive;
                rbInactive.Checked = !entity.IsActive;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            txtUserName.Text = String.Empty;
            txtUserId.Text = String.Empty;
            txtContact.Text = String.Empty;
            cbRole.SelectedIndex = 0;
            txtPwd.Text = String.Empty;
            txtRPwd.Text = String.Empty;
            rbActive.Checked = false;
            rbInactive.Checked = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter User Name");
                txtUserName.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtContact.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Contact No");
                txtContact.Focus();
                return;
            }
            if (String.IsNullOrEmpty(cbRole.SelectedValue.ToString().Trim()))
            {
                CommonFunc.DisplayErrorMessage("Select Role");
                cbRole.Focus();
                return;
            }
            if (!(rbActive.Checked || rbInactive.Checked))
            {
                CommonFunc.DisplayErrorMessage("Select Status");
                rbActive.Focus();
                return;
            }
            if (String.IsNullOrEmpty(txtPwd.Text.Trim()))
            {
                CommonFunc.DisplayErrorMessage("Enter Password");
                txtPwd.Focus();
                return;
            }

            if (txtPwd.Text != txtRPwd.Text)
            {
                CommonFunc.DisplayErrorMessage("Password and Re-type Password are not same");
                txtPwd.Focus();
                return;
            }
            UserEntity entity = new UserEntity();
            entity.DMLType = 'I';
            if (!String.IsNullOrEmpty(txtUserId.Text.Trim()))
            {
                entity.UserID = Convert.ToInt32(txtUserId.Text);
                entity.DMLType = 'U';
            }
            entity.UserName = txtUserName.Text.Trim();
            entity.ContactNo = txtContact.Text.Trim();
            entity.UserPassword = txtRPwd.Text.Trim();
            entity.RoleId = Convert.ToInt32(cbRole.SelectedValue.ToString());
            entity.IsActive = rbActive.Checked ? true : false;

            UserDal dal = new UserDal();
            AlertMsgInfo alertMsgInfo = new AlertMsgInfo();
            dal.SaveUsers(entity, alertMsgInfo);
            if (alertMsgInfo.SaveStatus)
            {
                ClearControls();
                CommonFunc.SaveMessage(alertMsgInfo.Message);
                fillGrid();
            }
            else
                CommonFunc.DisplayErrorMessage(alertMsgInfo.Message);
        }
    }
}
