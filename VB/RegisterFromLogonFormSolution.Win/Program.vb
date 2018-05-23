Imports Microsoft.VisualBasic
Imports System
Imports System.Configuration
Imports System.Windows.Forms

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Security
Imports DevExpress.ExpressApp.Win
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl

Namespace RegisterFromLogonFormSolution.Win
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		<STAThread> _
		Shared Sub Main()
#If EASYTEST Then
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register()
#End If
			'Demo AppServer starter
			ApplicationServerService.ApplicationServerService.StartSingle(False)

			Application.EnableVisualStyles()
			Application.SetCompatibleTextRenderingDefault(False)
			EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached
			Dim winApplication As New RegisterFromLogonFormSolutionWindowsFormsApplication()
#If EASYTEST Then
			If ConfigurationManager.ConnectionStrings("EasyTestConnectionString") IsNot Nothing Then
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings("EasyTestConnectionString").ConnectionString
			End If
#End If
			If ConfigurationManager.ConnectionStrings("ConnectionString") IsNot Nothing Then
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
				winApplication.Security = New SecurityStrategyProxy(winApplication.ConnectionString)
			End If
			Try
				' Uncomment this line when using the Middle Tier application server:
				Dim TempMiddleTierClientApplicationConfigurator As DevExpress.ExpressApp.MiddleTier.MiddleTierClientApplicationConfigurator = New DevExpress.ExpressApp.MiddleTier.MiddleTierClientApplicationConfigurator(winApplication)
				winApplication.Setup()
				winApplication.ShowViewStrategy = New ShowInSingleWindowStrategy(winApplication)
				winApplication.Start()
			Catch e As Exception
				winApplication.HandleException(e)
			End Try
		End Sub
	End Class
End Namespace
