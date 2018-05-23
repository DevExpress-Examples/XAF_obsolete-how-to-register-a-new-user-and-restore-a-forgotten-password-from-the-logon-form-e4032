using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace ApplicationServerService {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            if (System.Diagnostics.Debugger.IsAttached) {
                ApplicationServerService service = new ApplicationServerService();
                service.Setup();
                service.Start();
                System.Windows.Forms.MessageBox.Show("Application Server service is started");
                return;
            }
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ApplicationServerService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
