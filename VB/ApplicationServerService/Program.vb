Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ServiceProcess
Imports System.Text

Namespace ApplicationServerService
	Friend NotInheritable Class Program
		''' <summary>
		''' The main entry point for the application.
		''' </summary>
		Private Sub New()
		End Sub
		Shared Sub Main()
			If System.Diagnostics.Debugger.IsAttached Then
				Dim service As New ApplicationServerService()
				service.Setup()
				service.Start()
				System.Windows.Forms.MessageBox.Show("Application Server service is started")
				Return
			End If
			Dim ServicesToRun() As ServiceBase
			ServicesToRun = New ServiceBase() { New ApplicationServerService() }
			ServiceBase.Run(ServicesToRun)
		End Sub
	End Class
End Namespace
