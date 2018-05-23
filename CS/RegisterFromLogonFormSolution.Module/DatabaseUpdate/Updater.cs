using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using System.Collections.Generic;

namespace RegisterFromLogonFormSolution.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            CreateAnonymousAccess();
            SecurityRole defaultRole = CreateDefaultRole();
            defaultRole.Save();

            #region Create Users for the Complex Security Strategy
            // If a user named 'Sam' doesn't exist in the database, create this user
            SecurityUser user1 = ObjectSpace.FindObject<SecurityUser>(new BinaryOperator("UserName", "Sam"));
            if (user1 == null) {
                user1 = ObjectSpace.CreateObject<RegisteredUser>();
                user1.UserName = "Sam";
                // Set a password if the standard authentication type is used
                user1.SetPassword("");
            }
            // If a user named 'John' doesn't exist in the database, create this user
            SecurityUser user2 = ObjectSpace.FindObject<SecurityUser>(new BinaryOperator("UserName", "John"));
            if (user2 == null) {
                user2 = ObjectSpace.CreateObject<RegisteredUser>();
                user2.UserName = "John";
                // Set a password if the standard authentication type is used
                user2.SetPassword("");
            }
            // If a role with the Administrators name doesn't exist in the database, create this role
            SecurityRole adminRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null) {
                adminRole = ObjectSpace.CreateObject<SecurityRole>();
                adminRole.Name = "Administrators";
            }
            // If a role with the Users name doesn't exist in the database, create this role
            SecurityRole userRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", "Users"));
            if (userRole == null) {
                userRole = ObjectSpace.CreateObject<SecurityRole>();
                userRole.Name = "Users";
            }
            adminRole.BeginUpdate();
            adminRole.CanEditModel = true;
            adminRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.FullAccess);
            adminRole.EndUpdate();
            // Save the Administrators role to the database
            adminRole.Save();

            userRole.BeginUpdate();
            userRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.FullAccess);
            userRole.Permissions.DenyRecursive(typeof(SecurityUser), SecurityOperations.FullAccess);
            userRole.Permissions.DenyRecursive(typeof(SecurityRole), SecurityOperations.FullAccess);
            userRole.Permissions.DenyRecursive(typeof(PermissionDescriptorBase), SecurityOperations.FullAccess);
            userRole.Permissions.DenyRecursive(typeof(IPermissionData), SecurityOperations.FullAccess);
            userRole.Permissions.DenyRecursive(typeof(TypePermissionDetails), SecurityOperations.FullAccess);
            userRole.EndUpdate();
            // Save the Users role to the database
            userRole.Save();
            // Add the Administrators role to the user1
            user1.Roles.Add(adminRole);
            // Add the Users role to the user2
            user2.Roles.Add(userRole);
            user2.Roles.Add(defaultRole);
            // Save the users to the database
            user1.Save();
            user2.Save();
            #endregion

        }

        private SecurityRole CreateDefaultRole() {
            SecurityRole defaultRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", "Default"));
            if (defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<SecurityRole>();
                defaultRole.Name = "Default";
                ObjectOperationPermissionData myDetailsPermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                myDetailsPermission.TargetType = typeof(SecurityUser);
                myDetailsPermission.Criteria = "[Oid] = CurrentUserId()";
                myDetailsPermission.AllowNavigate = true;
                myDetailsPermission.AllowRead = true;
                myDetailsPermission.Save();
                defaultRole.PersistentPermissions.Add(myDetailsPermission);
                MemberOperationPermissionData userMembersPermission = ObjectSpace.CreateObject<MemberOperationPermissionData>();
                userMembersPermission.TargetType = typeof(SecurityUser);
                userMembersPermission.Members = "ChangePasswordOnFirstLogon, StoredPassword";
                userMembersPermission.AllowWrite = true;
                userMembersPermission.Save();
                defaultRole.PersistentPermissions.Add(userMembersPermission);
                ObjectOperationPermissionData defaultRolePermission = ObjectSpace.CreateObject<ObjectOperationPermissionData>();
                defaultRolePermission.TargetType = typeof(SecurityRole);
                defaultRolePermission.Criteria = "[Name] = 'Default'";
                defaultRolePermission.AllowNavigate = true;
                defaultRolePermission.AllowRead = true;
                defaultRolePermission.Save();
                defaultRole.PersistentPermissions.Add(defaultRolePermission);
                TypeOperationPermissionData auditDataItemPermission = ObjectSpace.CreateObject<TypeOperationPermissionData>();
                auditDataItemPermission.TargetType = typeof(AuditDataItemPersistent);
                auditDataItemPermission.AllowRead = true;
                auditDataItemPermission.AllowWrite = true;
                auditDataItemPermission.AllowCreate = true;
                auditDataItemPermission.Save();
                defaultRole.PersistentPermissions.Add(auditDataItemPermission);
            }
            return defaultRole;
        }
        private void CreateAnonymousAccess() {
            SecurityRole anonymousRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", SecurityStrategy.AnonymousUserName));
            if (anonymousRole == null) {
                anonymousRole = ObjectSpace.CreateObject<SecurityRole>();
                anonymousRole.Name = SecurityStrategy.AnonymousUserName;
                anonymousRole.BeginUpdate();
                //anonymousRole.Permissions[typeof(RegisteredUser)].Grant(SecurityOperations.Read);
                anonymousRole.Permissions[typeof(RegisteredUser)].Grant(SecurityOperations.FullAccess);
                anonymousRole.Permissions[typeof(SecurityRole)].Grant(SecurityOperations.FullAccess);
                anonymousRole.EndUpdate();
                anonymousRole.Save();
            }

            SecurityUser anonymousUser = ObjectSpace.FindObject<SecurityUser>(new BinaryOperator("UserName", SecurityStrategy.AnonymousUserName));
            if (anonymousUser == null) {
                anonymousUser = ObjectSpace.CreateObject<RegisteredUser>();
                anonymousUser.UserName = SecurityStrategy.AnonymousUserName;
                anonymousUser.IsActive = true;
                anonymousUser.SetPassword("");
                anonymousUser.Roles.Add(anonymousRole);
                anonymousUser.Save();
            }
        }

    }

}
