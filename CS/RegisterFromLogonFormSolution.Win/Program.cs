using System;
using System.Configuration;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;

namespace RegisterFromLogonFormSolution.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            //Demo AppServer starter
            ApplicationServerService.ApplicationServerService.StartSingle(false);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            RegisterFromLogonFormSolutionWindowsFormsApplication winApplication = new RegisterFromLogonFormSolutionWindowsFormsApplication();
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                winApplication.Security = new SecurityStrategyProxy(winApplication.ConnectionString);
            }
            try {
                // Uncomment this line when using the Middle Tier application server:
                new DevExpress.ExpressApp.MiddleTier.MiddleTierClientApplicationConfigurator(winApplication);
                winApplication.Setup();
                winApplication.ShowViewStrategy = new ShowInSingleWindowStrategy(winApplication);
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
