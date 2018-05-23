using System;
using System.ComponentModel;

using DevExpress.ExpressApp;
using WinWebSolution.Module.Web.Controllers;

namespace RegisterFromLogonFormSolution.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class RegisterFromLogonFormSolutionAspNetModule : ModuleBase {
        public RegisterFromLogonFormSolutionAspNetModule() {
            InitializeComponent();
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.CreateCustomLogonWindowControllers += application_CreateCustomLogonWindowControllers;
        }
        private void application_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            XafApplication app = (XafApplication)sender;
            e.Controllers.Add(app.CreateController<WebLogonActionParametersController>());
        }
    }
}
