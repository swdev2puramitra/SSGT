using System;
using System.Drawing;
using System.Windows.Forms;
using FireWorks.UI;

namespace FireWorks
{
    public class BaseForm : Form
    {
        public BaseForm()
        {
            try
            {
                this.Font = UiTheme.AppFont;
                this.BackColor = UiTheme.Background;
                this.AutoScaleMode = AutoScaleMode.Font;
            }
            catch { }
        }

        protected void ApplyGridStyle(DataGridView dgv)
        {
            UiHelpers.ApplyGridStyle(dgv);
        }

        protected void ApplyButtonStyle(Button btn, bool primary = false)
        {
            UiHelpers.ApplyElegantStyle(btn, primary);
        }
    }
}
