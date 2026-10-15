using FireWorks.Util;
using FireWorksDal.DataLayer;
using System;
using System.Windows.Forms;

namespace FireWorks
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // First-time setup
            try
            {
                SubscriptionManager.Initialize();
            }
            catch (Exception)
            {
                CommonFunc.DisplayErrorMessage("Unable to initialize subscription manager");
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // initialize automatic UI styler to apply consistent styles to forms at runtime
            try { FireWorks.UI.UiAutoStyler.Initialize(); } catch { }
            // Ensure the app icon is loaded and applied to forms
            FireWorks.UI.UiAutoStyler.InitializeAppIcon();
            UserDal dal = new UserDal();
            dal.FillShopDetails();

            Application.Run(new login());
        }
    }
}
