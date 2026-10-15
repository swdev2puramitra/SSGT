using FireWorks.Util;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FireWorks
{
    public partial class CustomerType : Form
    {
        public CustomerType()
        {
            InitializeComponent();
            CommonFunc.fillCustomer(cbCustomer);
            cbCustomer.Focus();            
        }

        private void cbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbCustomer.SelectedIndex > 0)
            {
                PriceList priceList = new PriceList(Convert.ToInt32(cbCustomer.SelectedValue))
                {
                    MdiParent = this.MdiParent
                };
                this.Close();
                priceList.Show();
            }
            else
            {
                CommonFunc.DisplayErrorMessage("Select Customer");
            }
        }
    }
}
