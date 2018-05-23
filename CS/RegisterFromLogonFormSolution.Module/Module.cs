using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using System.Reflection;


namespace RegisterFromLogonFormSolution.Module {
    public sealed partial class RegisterFromLogonFormSolutionModule : ModuleBase {
        public RegisterFromLogonFormSolutionModule() {
            InitializeComponent();
        }
        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            List<Type> result = new List<Type>(base.GetDeclaredExportedTypes());
            result.Add(typeof(RegisteredUser));
            return result;
        }
    }
}
