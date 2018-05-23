Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

Imports DevExpress.ExpressApp
Imports System.Reflection


Namespace RegisterFromLogonFormSolution.Module
	Public NotInheritable Partial Class RegisterFromLogonFormSolutionModule
		Inherits ModuleBase
		Public Sub New()
			InitializeComponent()
		End Sub
		Protected Overrides Function GetDeclaredExportedTypes() As IEnumerable(Of Type)
			Dim result As New List(Of Type)(MyBase.GetDeclaredExportedTypes())
			result.Add(GetType(RegisteredUser))
			Return result
		End Function
	End Class
End Namespace
