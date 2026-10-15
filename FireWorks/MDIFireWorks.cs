using FireWorks.Util;
using FireWorksDal.DataLayer;
using FireWorksEntity;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class MDIFireWorks : Form
    {
        // Inside the MDIFireWorks class, replace / update the constructor implementation with this:
        public MDIFireWorks()
        {
            InitializeComponent();
            CreateMenuWithEventAndKey();
        }       

        private void CreateMenuWithEventAndKey()
        {
            //Creating object of MenuStrip class
            MenuStrip MnuStrip = new MenuStrip();

            //Placing the control to the Form
            this.Controls.Add(MnuStrip);

            // apply consistent menu styling
            FireWorks.UI.UiHelpers.ApplyMenuStyle(MnuStrip);

            UserDal dal = new UserDal();
            DataTable table = dal.GetMenus();

            DataView view = new DataView(table);
            view.RowFilter = "ParentID = 0";
            foreach (DataRowView dr in view)
            {
                ToolStripMenuItem MnuStripItem = null;
                if (dr["FormName"].ToString() != "")
                    MnuStripItem = new ToolStripMenuItem(dr["MenuName"].ToString(), null, new EventHandler(ChildClick), dr["FormName"].ToString());
                else
                    MnuStripItem = new ToolStripMenuItem(dr["MenuName"].ToString());
                SubMenu(MnuStripItem, dr["MenuId"].ToString(), table);
                MnuStrip.Items.Add(MnuStripItem);
            }
            ToolStripMenuItem SSMenu = new ToolStripMenuItem("Logout", null, new EventHandler(ChildClick), "LogOut");
            MnuStrip.Items.Add(SSMenu);

            // The Form.MainMenuStrip property determines the merge target.
            this.MainMenuStrip = MnuStrip;
        }

        public void SubMenu(ToolStripMenuItem mnu, string submenu, DataTable dtchild)
        {
            DataView view = new DataView(dtchild);
            view.RowFilter = "ParentID = " + submenu;
            foreach (DataRowView dr in view)
            {
                ToolStripMenuItem SSMenu = new ToolStripMenuItem(dr["MenuName"].ToString(), null, new EventHandler(ChildClick), dr["FormName"].ToString());
                mnu.DropDownItems.Add(SSMenu);
            }
        }

        private void ChildClick(object sender, EventArgs e)
        {
            //MessageBox.Show(string.Concat("You have Clicked ", sender.ToString(), " Menu"), "Menu Items Event", MessageBoxButtons.OK, MessageBoxIcon.Information);
            try
            {
                Assembly frmAssembly = Assembly.LoadFile(Application.ExecutablePath);
                ToolStripItem item = (ToolStripItem)sender;

                foreach (Type type in frmAssembly.GetTypes())
                {
                    // support forms that may inherit from a custom BaseForm as well as direct Form
                    if (type.IsSubclassOf(typeof(Form)))
                    {
                        if (string.Equals(type.Name, item.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            Form frmShow = (Form)frmAssembly.CreateInstance(type.ToString());
                            // close existing MDI children
                            foreach (Form form in this.MdiChildren)
                            {
                                form.Close();
                            }
                            if (item.Name != "LogOut")
                                frmShow.MdiParent = this;
                            else
                            {
                                this.Close();
                            }
                            frmShow.Show();
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    stringBuilder.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException notFoundException)
                    {
                        if (!string.IsNullOrEmpty(notFoundException.FusionLog))
                        {
                            stringBuilder.AppendLine("Fusion log: ");
                            stringBuilder.AppendLine(notFoundException.FusionLog);
                        }
                    }
                    stringBuilder.AppendLine();
                }

                Errorlog.WriteToErrorLog(ex, UserSessionEntity.UserID, stringBuilder.ToString(), "Class : MDIFireWorks, Method : ChildClick");
            }
        }

        private void MDIFireWorks_FormClosed(object sender, FormClosedEventArgs e)
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

        private void MDIFireWorks_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "Are you sure you want to Exit";
            string caption = CommonFunc.ApplicationTitile;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(this, message, caption, buttons,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1,
                MessageBoxOptions.RightAlign);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                // Closes the parent form. 

                //this.Close();
            }

        }

        private void MDIFireWorks_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunc.ApplicationTitile;

            try
            {
                // Style the status strip for a modern look
                FireWorks.UI.UiHelpers.ApplyStatusStripStyle(this.statusStrip);

                // Add additional status items (User, Role, Version) without modifying Designer file
                if (!this.statusStrip.Items.Cast<ToolStripItem>().Any(i => i.Name == "toolStripUser"))
                {
                    var userItem = new ToolStripStatusLabel { Name = "toolStripUser", Text = $"User: {UserSessionEntity.UserName ?? "-"}", Spring = false };
                    var roleItem = new ToolStripStatusLabel { Name = "toolStripRole", Text = $"Role: {UserSessionEntity.RoleId}", Spring = false };
                    var versionItem = new ToolStripStatusLabel { Name = "toolStripVersion", Text = $"v{Application.ProductVersion}", Spring = false };

                    // Insert user info to the left of the existing date/time labels
                    //this.statusStrip.Items.Insert(0, versionItem);
                    //this.statusStrip.Items.Insert(0, roleItem);
                    this.statusStrip.Items.Insert(0, userItem);
                }
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripDate.Text = DateTime.Now.ToShortDateString();
            toolStripTime.Text = DateTime.Now.ToLongTimeString();
        }

    }
}
