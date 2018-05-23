Imports Microsoft.VisualBasic
Imports System
Imports System.ComponentModel

Imports DevExpress.ExpressApp
Imports WinWebSolution.Module.Web.Controllers

Namespace RegisterFromLogonFormSolution.Module.Web
	<ToolboxItemFilter("Xaf.Platform.Web")> _
	Public NotInheritable Partial Class RegisterFromLogonFormSolutionAspNetModule
		Inherits ModuleBase
		Public Sub New()
			InitializeComponent()
		End Sub
		Public Overrides Overloads Sub Setup(ByVal application As XafApplication)
			MyBase.Setup(application)
			AddHandler application.CreateCustomLogonWindowControllers, AddressOf application_CreateCustomLogonWindowControllers
		End Sub
		Private Sub application_CreateCustomLogonWindowControllers(ByVal sender As Object, ByVal e As CreateCustomLogonWindowControllersEventArgs)
			Dim app As XafApplication = CType(sender, XafApplication)
			e.Controllers.Add(app.CreateController(Of WebLogonActionParametersController)())
		End Sub
	End Class
End Namespace
