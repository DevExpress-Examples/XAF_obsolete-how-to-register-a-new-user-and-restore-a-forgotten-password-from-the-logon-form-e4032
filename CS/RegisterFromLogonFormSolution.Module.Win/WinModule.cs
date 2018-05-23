using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using DevExpress.ExpressApp;
using WinWebSolution.Module.Controllers;

namespace RegisterFromLogonFormSolution.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class RegisterFromLogonFormSolutionWindowsFormsModule : ModuleBase {
        public RegisterFromLogonFormSolutionWindowsFormsModule() {
            InitializeComponent();
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
        }
        private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            XafApplication app = (XafApplication)sender;
            e.Controllers.Add(app.CreateController<LogonActionParametersController>());
        }
    }
}
