Imports Microsoft.VisualBasic
Imports System

Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Updating
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.ExpressApp.Security
Imports System.Collections.Generic

Namespace RegisterFromLogonFormSolution.Module.DatabaseUpdate
	Public Class Updater
		Inherits ModuleUpdater
		Public Sub New(ByVal objectSpace As IObjectSpace, ByVal currentDBVersion As Version)
			MyBase.New(objectSpace, currentDBVersion)
		End Sub
		Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
			MyBase.UpdateDatabaseAfterUpdateSchema()

			CreateAnonymousAccess()
			Dim defaultRole As SecurityRole = CreateDefaultRole()
			defaultRole.Save()

'			#Region "Create Users for the Complex Security Strategy"
			' If a user named 'Sam' doesn't exist in the database, create this user
			Dim user1 As SecurityUser = ObjectSpace.FindObject(Of SecurityUser)(New BinaryOperator("UserName", "Sam"))
			If user1 Is Nothing Then
				user1 = ObjectSpace.CreateObject(Of RegisteredUser)()
				user1.UserName = "Sam"
				' Set a password if the standard authentication type is used
				user1.SetPassword("")
			End If
			' If a user named 'John' doesn't exist in the database, create this user
			Dim user2 As SecurityUser = ObjectSpace.FindObject(Of SecurityUser)(New BinaryOperator("UserName", "John"))
			If user2 Is Nothing Then
				user2 = ObjectSpace.CreateObject(Of RegisteredUser)()
				user2.UserName = "John"
				' Set a password if the standard authentication type is used
				user2.SetPassword("")
			End If
			' If a role with the Administrators name doesn't exist in the database, create this role
			Dim adminRole As SecurityRole = ObjectSpace.FindObject(Of SecurityRole)(New BinaryOperator("Name", "Administrators"))
			If adminRole Is Nothing Then
				adminRole = ObjectSpace.CreateObject(Of SecurityRole)()
				adminRole.Name = "Administrators"
			End If
			' If a role with the Users name doesn't exist in the database, create this role
			Dim userRole As SecurityRole = ObjectSpace.FindObject(Of SecurityRole)(New BinaryOperator("Name", "Users"))
			If userRole Is Nothing Then
				userRole = ObjectSpace.CreateObject(Of SecurityRole)()
				userRole.Name = "Users"
			End If
			adminRole.BeginUpdate()
			adminRole.CanEditModel = True
			adminRole.Permissions.GrantRecursive(GetType(Object), SecurityOperations.FullAccess)
			adminRole.EndUpdate()
			' Save the Administrators role to the database
			adminRole.Save()

			userRole.BeginUpdate()
			userRole.Permissions.GrantRecursive(GetType(Object), SecurityOperations.FullAccess)
			userRole.Permissions.DenyRecursive(GetType(SecurityUser), SecurityOperations.FullAccess)
			userRole.Permissions.DenyRecursive(GetType(SecurityRole), SecurityOperations.FullAccess)
			userRole.Permissions.DenyRecursive(GetType(PermissionDescriptorBase), SecurityOperations.FullAccess)
			userRole.Permissions.DenyRecursive(GetType(IPermissionData), SecurityOperations.FullAccess)
			userRole.Permissions.DenyRecursive(GetType(TypePermissionDetails), SecurityOperations.FullAccess)
			userRole.EndUpdate()
			' Save the Users role to the database
			userRole.Save()
			' Add the Administrators role to the user1
			user1.Roles.Add(adminRole)
			' Add the Users role to the user2
			user2.Roles.Add(userRole)
			user2.Roles.Add(defaultRole)
			' Save the users to the database
			user1.Save()
			user2.Save()
'			#End Region

		End Sub

		Private Function CreateDefaultRole() As SecurityRole
			Dim defaultRole As SecurityRole = ObjectSpace.FindObject(Of SecurityRole)(New BinaryOperator("Name", "Default"))
			If defaultRole Is Nothing Then
				defaultRole = ObjectSpace.CreateObject(Of SecurityRole)()
				defaultRole.Name = "Default"
				Dim myDetailsPermission As ObjectOperationPermissionData = ObjectSpace.CreateObject(Of ObjectOperationPermissionData)()
				myDetailsPermission.TargetType = GetType(SecurityUser)
				myDetailsPermission.Criteria = "[Oid] = CurrentUserId()"
				myDetailsPermission.AllowNavigate = True
				myDetailsPermission.AllowRead = True
				myDetailsPermission.Save()
				defaultRole.PersistentPermissions.Add(myDetailsPermission)
				Dim userMembersPermission As MemberOperationPermissionData = ObjectSpace.CreateObject(Of MemberOperationPermissionData)()
				userMembersPermission.TargetType = GetType(SecurityUser)
				userMembersPermission.Members = "ChangePasswordOnFirstLogon, StoredPassword"
				userMembersPermission.AllowWrite = True
				userMembersPermission.Save()
				defaultRole.PersistentPermissions.Add(userMembersPermission)
				Dim defaultRolePermission As ObjectOperationPermissionData = ObjectSpace.CreateObject(Of ObjectOperationPermissionData)()
				defaultRolePermission.TargetType = GetType(SecurityRole)
				defaultRolePermission.Criteria = "[Name] = 'Default'"
				defaultRolePermission.AllowNavigate = True
				defaultRolePermission.AllowRead = True
				defaultRolePermission.Save()
				defaultRole.PersistentPermissions.Add(defaultRolePermission)
				Dim auditDataItemPermission As TypeOperationPermissionData = ObjectSpace.CreateObject(Of TypeOperationPermissionData)()
				auditDataItemPermission.TargetType = GetType(AuditDataItemPersistent)
				auditDataItemPermission.AllowRead = True
				auditDataItemPermission.AllowWrite = True
				auditDataItemPermission.AllowCreate = True
				auditDataItemPermission.Save()
				defaultRole.PersistentPermissions.Add(auditDataItemPermission)
			End If
			Return defaultRole
		End Function
		Private Sub CreateAnonymousAccess()
			Dim anonymousRole As SecurityRole = ObjectSpace.FindObject(Of SecurityRole)(New BinaryOperator("Name", SecurityStrategy.AnonymousUserName))
			If anonymousRole Is Nothing Then
				anonymousRole = ObjectSpace.CreateObject(Of SecurityRole)()
				anonymousRole.Name = SecurityStrategy.AnonymousUserName
				anonymousRole.BeginUpdate()
				'anonymousRole.Permissions[typeof(RegisteredUser)].Grant(SecurityOperations.Read);
				anonymousRole.Permissions(GetType(RegisteredUser)).Grant(SecurityOperations.FullAccess)
				anonymousRole.Permissions(GetType(SecurityRole)).Grant(SecurityOperations.FullAccess)
				anonymousRole.EndUpdate()
				anonymousRole.Save()
			End If

			Dim anonymousUser As SecurityUser = ObjectSpace.FindObject(Of SecurityUser)(New BinaryOperator("UserName", SecurityStrategy.AnonymousUserName))
			If anonymousUser Is Nothing Then
				anonymousUser = ObjectSpace.CreateObject(Of RegisteredUser)()
				anonymousUser.UserName = SecurityStrategy.AnonymousUserName
				anonymousUser.IsActive = True
				anonymousUser.SetPassword("")
				anonymousUser.Roles.Add(anonymousRole)
				anonymousUser.Save()
			End If
		End Sub

	End Class

End Namespace
