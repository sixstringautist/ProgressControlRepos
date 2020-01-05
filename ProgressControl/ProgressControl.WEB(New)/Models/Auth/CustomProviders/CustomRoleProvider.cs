using System.Web.Security;
namespace ProgressControl.WEB_New_.Models.Auth.CustomProviders
{
    public class CustomRoleProvider : RoleProvider
    {
        public CustomRoleProvider() : base()
        {

        }

        public override string ApplicationName { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var tmp = new Crutch())
            {
                tmp.AddUsersToRoles(usernames, roleNames);
            }
        }

        public override void CreateRole(string roleName)
        {
            using (var tmp = new Crutch())
            {
                tmp.CreateRole(roleName);
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (var tmp = new Crutch())
            {
                return tmp.DeleteRole(roleName, throwOnPopulatedRole);
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            using (var tmp = new Crutch())
            {
                return tmp.FindUsersInRole(roleName, usernameToMatch);
            }
        }

        public override string[] GetAllRoles()
        {
            using (var tmp = new Crutch())
            {
                return tmp.GetAllRoles();
            }

        }

        public override string[] GetRolesForUser(string username)
        {
            using (var tmp = new Crutch())
            {
                return tmp.GetRolesForUser(username);
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (var tmp = new Crutch())
            {
                return tmp.GetUsersInRole(roleName);
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var tmp = new Crutch())
            {
                return tmp.IsUserInRole(username, roleName);
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var tmp = new Crutch())
            {
                tmp.RemoveUsersFromRoles(usernames, roleNames);
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (var tmp = new Crutch())
            {
                return tmp.RoleExists(roleName);
            }
        } 
    }
}
