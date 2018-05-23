Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports DevExpress.ExpressApp.Web

Namespace RegisterFromLogonFormSolution.Web
	Partial Public Class RegisterFromLogonFormSolutionAspNetApplication
		Inherits WebApplication
		Private module1 As DevExpress.ExpressApp.SystemModule.SystemModule
		Private module2 As DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule
		Private module3 As RegisterFromLogonFormSolution.Module.RegisterFromLogonFormSolutionModule
		Private module4 As RegisterFromLogonFormSolution.Module.Web.RegisterFromLogonFormSolutionAspNetModule
		Private securityModule1 As DevExpress.ExpressApp.Security.SecurityModule
		Private module6 As DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule
		Private module5 As DevExpress.ExpressApp.Validation.ValidationModule

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub RegisterFromLogonFormSolutionAspNetApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs) Handles MyBase.DatabaseVersionMismatch
#If EASYTEST Then
			e.Updater.Update()
			e.Handled = True
#Else
			'if (System.Diagnostics.Debugger.IsAttached) {
			'    e.Updater.Update();
			'    e.Handled = true;
			'} else {
				Dim message As String = "The application cannot connect to the specified database, because the latter doesn't exist or its version is older than that of the application." & Constants.vbCrLf & "This error occurred  because the automatic database update was disabled when the application was started without debugging." & Constants.vbCrLf & "To avoid this error, you should either start the application under Visual Studio in debug mode, or modify the " & "source code of the 'DatabaseVersionMismatch' event handler to enable automatic database update, " & "or manually create a database using the 'DBUpdater' tool." & Constants.vbCrLf & "Anyway, refer to the following help topics for more detailed information:" & Constants.vbCrLf & "'Update Application and Database Versions' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument2795.htm" & Constants.vbCrLf & "'Database Security References' at http://www.devexpress.com/Help/?document=ExpressApp/CustomDocument3237.htm" & Constants.vbCrLf & "If this doesn't help, please contact our Support Team at http://www.devexpress.com/Support/Center/"

				If e.CompatibilityError IsNot Nothing AndAlso e.CompatibilityError.Exception IsNot Nothing Then
					message &= Constants.vbCrLf & Constants.vbCrLf & "Inner exception: " & e.CompatibilityError.Exception.Message
				End If
				Throw New InvalidOperationException(message)
			'}
#End If
		End Sub

		Private Sub InitializeComponent()
			Me.module1 = New DevExpress.ExpressApp.SystemModule.SystemModule()
			Me.module2 = New DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule()
			Me.module3 = New RegisterFromLogonFormSolution.Module.RegisterFromLogonFormSolutionModule()
			Me.module4 = New RegisterFromLogonFormSolution.Module.Web.RegisterFromLogonFormSolutionAspNetModule()
			Me.module5 = New DevExpress.ExpressApp.Validation.ValidationModule()
			Me.module6 = New DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule()
			Me.securityModule1 = New DevExpress.ExpressApp.Security.SecurityModule()
			CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
			' 
			' module5
			' 
			Me.module5.AllowValidationDetailsAccess = True
			' 
			' RegisterFromLogonFormSolutionAspNetApplication
			' 
			Me.ApplicationName = "RegisterFromLogonFormSolution"
			Me.Modules.Add(Me.module1)
			Me.Modules.Add(Me.module2)
			Me.Modules.Add(Me.module6)
			Me.Modules.Add(Me.securityModule1)
			Me.Modules.Add(Me.module3)
			Me.Modules.Add(Me.module4)
			Me.Modules.Add(Me.module5)
'			Me.DatabaseVersionMismatch += New System.EventHandler(Of DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs)(Me.RegisterFromLogonFormSolutionAspNetApplication_DatabaseVersionMismatch);
			CType(Me, System.ComponentModel.ISupportInitialize).EndInit()

		End Sub
	End Class
End Namespace
