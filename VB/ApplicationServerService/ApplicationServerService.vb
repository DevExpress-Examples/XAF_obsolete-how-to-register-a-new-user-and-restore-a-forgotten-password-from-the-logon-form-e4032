Imports Microsoft.VisualBasic
Imports System
Imports System.Configuration
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.ServiceProcess
Imports System.Text
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Persistent.Base
Imports DevExpress.ExpressApp.MiddleTier
Imports RegisterFromLogonFormSolution.Module

Namespace ApplicationServerService
	Partial Public Class ApplicationServerService
		Inherits System.ServiceProcess.ServiceBase
		#Region "Local in-app starter"
		Private Shared domain As AppDomain
		Private Shared starter As ApplicationServerService
		Public Shared Sub StartSingle(ByVal isWeb As Boolean)
			If isWeb Then
				Dim domainSetup As New AppDomainSetup()
				domainSetup.ApplicationBase = AppDomain.CurrentDomain.RelativeSearchPath
				domain = AppDomain.CreateDomain("ServerDomain", Nothing, domainSetup)
			Else
				domain = AppDomain.CreateDomain("ServerDomain")
			End If
			Dim thisType As Type = GetType(ApplicationServerService)
			starter = CType(domain.CreateInstanceAndUnwrap(thisType.Assembly.FullName, thisType.FullName), ApplicationServerService)
			starter.Setup(ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString, ConfigurationManager.ConnectionStrings("ServerConnectionString").ConnectionString)
			starter.Start()
		End Sub
		Public Shared Sub StopSingle()
			If domain IsNot Nothing Then
				AppDomain.Unload(domain)
			End If
		End Sub
		Public Sub Start()
			applicationServer.Start()
		End Sub
		#End Region

		Private applicationServer As ApplicationServer
		Private Sub serverApplication_DatabaseVersionMismatch(ByVal sender As Object, ByVal e As DatabaseVersionMismatchEventArgs)
			e.Updater.Update()
			e.Handled = True
		End Sub
		Protected Overrides Sub OnStart(ByVal args() As String)
			Setup()
			applicationServer.Start()
		End Sub
		Protected Overrides Sub OnStop()
			applicationServer.Stop()
		End Sub
		Public Sub New()
			InitializeComponent()
		End Sub
		Public Sub Setup()
			Setup(ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString, ConfigurationManager.ConnectionStrings("ServerConnectionString").ConnectionString)
		End Sub
		Private Sub Setup(ByVal _ConnectionString As String, ByVal _ServerConnectionString As String)
			Dim serverApplication As New ServerApplication()

			' Change the ServerApplication.ApplicationName property value. It should be the same as your client application name. 
			serverApplication.ApplicationName = "RegisterFromLogonFormSolution"
			AddHandler serverApplication.DatabaseVersionMismatch, AddressOf serverApplication_DatabaseVersionMismatch

			' Add your client application's modules to the ServerApplication.Modules collection here. 
			serverApplication.Modules.Add(New DevExpress.ExpressApp.SystemModule.SystemModule())
			serverApplication.Modules.Add(New DevExpress.ExpressApp.Security.SecurityModule())
			serverApplication.Modules.Add(New DevExpress.ExpressApp.Validation.ValidationModule())
			serverApplication.Modules.Add(New DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule())
			serverApplication.Modules.Add(New DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule())
			serverApplication.Modules.Add(New RegisterFromLogonFormSolution.Module.RegisterFromLogonFormSolutionModule())
			serverApplication.Modules.Add(New RegisterFromLogonFormSolution.Module.Win.RegisterFromLogonFormSolutionWindowsFormsModule())
			serverApplication.Modules.Add(New RegisterFromLogonFormSolution.Module.Web.RegisterFromLogonFormSolutionAspNetModule())

			serverApplication.ConnectionString = _ServerConnectionString

			' Configure the authentication type and security strategy here.
			'AuthenticationActiveDirectory authentication = new AuthenticationActiveDirectory<SecuritySimpleUser>();
			'authentication.CreateUserAutomatically = true;
			'serverApplication.Security = new SecurityStrategySimple(typeof(SecuritySimpleUser), authentication);
			Dim authentication As New AuthenticationStandard()
			serverApplication.Security = New SecurityStrategyComplex(GetType(RegisteredUser), GetType(SecurityRole), authentication)

			serverApplication.Setup()
			serverApplication.CheckCompatibility()

			applicationServer = New ApplicationServer(_ConnectionString, "ApplicationServerService", _ServerConnectionString)
			applicationServer.ObjectSpaceProvider = serverApplication.ObjectSpaceProvider
			applicationServer.Security = serverApplication.Security
			applicationServer.SecurityService = New ServerSecurityStrategyService(authentication)

			AddHandler applicationServer.CustomHandleException, Function(sender, e) AnonymousMethod1(sender, e)
		End Sub
		
		Private Function AnonymousMethod1(ByVal sender As Object, ByVal e As DevExpress.ExpressApp.MiddleTier.CustomHandleServiceExceptionEventArgs) As Boolean
			Tracing.Tracer.LogError(e.Exception)
			e.Handled = False
			Return True
		End Function
	End Class
End Namespace
