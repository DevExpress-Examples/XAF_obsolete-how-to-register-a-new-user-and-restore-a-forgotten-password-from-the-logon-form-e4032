Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ComponentModel

Imports DevExpress.ExpressApp
Imports WinWebSolution.Module.Controllers

Namespace RegisterFromLogonFormSolution.Module.Win
	<ToolboxItemFilter("Xaf.Platform.Win")> _
	Public NotInheritable Partial Class RegisterFromLogonFormSolutionWindowsFormsModule
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
			e.Controllers.Add(app.CreateController(Of LogonActionParametersController)())
		End Sub
	End Class
End Namespace
