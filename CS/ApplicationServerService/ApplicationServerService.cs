using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.MiddleTier;
using RegisterFromLogonFormSolution.Module;

namespace ApplicationServerService {
    public partial class ApplicationServerService : System.ServiceProcess.ServiceBase {
        #region Local in-app starter
        private static AppDomain domain;
        private static ApplicationServerService starter;
        public static void StartSingle(bool isWeb) {
            if (isWeb) {
                AppDomainSetup domainSetup = new AppDomainSetup();
                domainSetup.ApplicationBase = AppDomain.CurrentDomain.RelativeSearchPath;
                domain = AppDomain.CreateDomain("ServerDomain", null, domainSetup);
            } else {
                domain = AppDomain.CreateDomain("ServerDomain");
            }
            Type thisType = typeof(ApplicationServerService);
            starter = (ApplicationServerService)domain.CreateInstanceAndUnwrap(thisType.Assembly.FullName, thisType.FullName);
            starter.Setup(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, ConfigurationManager.ConnectionStrings["ServerConnectionString"].ConnectionString);
            starter.Start();
        }
        public static void StopSingle() {
            if (domain != null) {
                AppDomain.Unload(domain);
            }
        }
        internal void Start() {
            applicationServer.Start();
        }
        #endregion

        private ApplicationServer applicationServer;
        private void serverApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
            e.Updater.Update();
            e.Handled = true;
        }
        protected override void OnStart(string[] args) {
            Setup();
            applicationServer.Start();
        }
        protected override void OnStop() {
            applicationServer.Stop();
        }
        public ApplicationServerService() {
            InitializeComponent();
        }
        internal void Setup() {
            Setup(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString, ConfigurationManager.ConnectionStrings["ServerConnectionString"].ConnectionString);
        }
        private void Setup(string _ConnectionString, string _ServerConnectionString) {
            ServerApplication serverApplication = new ServerApplication();

            // Change the ServerApplication.ApplicationName property value. It should be the same as your client application name. 
            serverApplication.ApplicationName = "RegisterFromLogonFormSolution";
            serverApplication.DatabaseVersionMismatch += new EventHandler<DatabaseVersionMismatchEventArgs>(serverApplication_DatabaseVersionMismatch);

            // Add your client application's modules to the ServerApplication.Modules collection here. 
            serverApplication.Modules.Add(new DevExpress.ExpressApp.SystemModule.SystemModule());
            serverApplication.Modules.Add(new DevExpress.ExpressApp.Security.SecurityModule());
            serverApplication.Modules.Add(new DevExpress.ExpressApp.Validation.ValidationModule());
            serverApplication.Modules.Add(new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule());
            serverApplication.Modules.Add(new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule());
            serverApplication.Modules.Add(new RegisterFromLogonFormSolution.Module.RegisterFromLogonFormSolutionModule());
            serverApplication.Modules.Add(new RegisterFromLogonFormSolution.Module.Win.RegisterFromLogonFormSolutionWindowsFormsModule());
            serverApplication.Modules.Add(new RegisterFromLogonFormSolution.Module.Web.RegisterFromLogonFormSolutionAspNetModule());

            serverApplication.ConnectionString = _ServerConnectionString;

            // Configure the authentication type and security strategy here.
            //AuthenticationActiveDirectory authentication = new AuthenticationActiveDirectory<SecuritySimpleUser>();
            //authentication.CreateUserAutomatically = true;
            //serverApplication.Security = new SecurityStrategySimple(typeof(SecuritySimpleUser), authentication);
            AuthenticationStandard authentication = new AuthenticationStandard();
            serverApplication.Security = new SecurityStrategyComplex(typeof(RegisteredUser), typeof(SecurityRole), authentication);

            serverApplication.Setup();
            serverApplication.CheckCompatibility();

            applicationServer = new ApplicationServer(_ConnectionString, "ApplicationServerService", _ServerConnectionString);
            applicationServer.ObjectSpaceProvider = serverApplication.ObjectSpaceProvider;
            applicationServer.Security = serverApplication.Security;
            applicationServer.SecurityService = new ServerSecurityStrategyService(authentication);

            applicationServer.CustomHandleException += delegate(object sender, DevExpress.ExpressApp.MiddleTier.CustomHandleServiceExceptionEventArgs e) {
                Tracing.Tracer.LogError(e.Exception);
                e.Handled = false;
            };
        }
    }
}
