using System;
using System.Linq;
using System.Web.Security;
using ProgressControl.WEB.Models.Auth.EF;
using ProgressControl.WEB.Models.Auth.Entities;
using System.Configuration.Provider;
using System.Web.Mvc;

namespace ProgressControl.WEB.Models.Auth.CustomProviders
{
    public class CustomRoleProvider : RoleProvider
    {
        private UserContext db;


        public CustomRoleProvider() :base()
        {
            this.db = DependencyResolver.Current.GetService<UserContext>();
        }
        public CustomRoleProvider(UserContext db): base()
        {
            this.db = db;
        }
        public override string ApplicationName { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (roleNames.Where(x => x == "" || x == null).Count() > 0)
                throw new ArgumentNullException("Role name cannot be null or empty.");
            if (roleNames.Where(x => !RoleExists(x) ? true : false).Count() > 0)
                throw new ProviderException("Role name not found.");
            if (roleNames.FirstOrDefault(x => x.Contains(",")) != null)
            {
                throw new ArgumentException("Role name contains \",\"");
            }
            if (usernames.FirstOrDefault(x => x.Contains(",")) != null)
            {
                throw new ArgumentException("Username contains \",\"");
            }

            if (usernames.Where(x => x == "").Count() > 0)
                throw new ArgumentException("Username cannot be empty.");

            if (usernames.FirstOrDefault(x => x == null) != null)
            {
                throw new ArgumentNullException("Username cannot be null.");
            }

            if (usernames.Where(x => db.Users.FirstOrDefault(z => z.UserName == x) == null).Count() > 0)
            {
                throw new ProviderException("Username not found.");
            }

            var tmp = from un in usernames
                      from rl in roleNames
                      where IsUserInRole(un, rl) == true
                      select un;
            if (tmp.Count() > 0)
                throw new ProviderException("User alredy in role");

            var users = db.Users.Where(x => usernames.Contains(x.UserName));
            var roles = db.Roles.Where(x => roleNames.Contains(x.RoleName)).ToList();

            foreach (var user in users)
            {
                roles.ForEach(x => user.Collection.Add(x));
            }
            db.SaveChanges();
        }

        public override void CreateRole(string roleName)
        {
            if (roleName == null)
                throw new ArgumentNullException("Role name cannot be null");
            if(roleName == "")
                throw new ArgumentException("Role name cannot be empty");

            if (RoleExists(roleName))
                throw new ProviderException("Role name alredy exists.");

            var newRole = new Role() { RoleName = roleName};
            db.Roles.Add(newRole);
            db.SaveChanges();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (roleName == null)
                throw new ArgumentNullException("Role name cannot be null");
            if (roleName == "")
                throw new ArgumentException("Role name cannot be empty");

            if (!RoleExists(roleName))
                throw new ProviderException("Role name not found.");

            var role = db.Roles.FirstOrDefault(x => x.RoleName == roleName);
            if (throwOnPopulatedRole)
            {
                if (role.Collection.Count() > 0)
                    throw new ProviderException($"{roleName} have one or more Users");
            }

            db.Roles.Remove(role);
            db.SaveChanges();
            role = db.Roles.FirstOrDefault(x => x.RoleName == role.RoleName);
            return role == null ? true : false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (roleName == null)
                throw new ArgumentNullException("Role name cannot be null");
            if(roleName == "")
                throw new ArgumentException("Role name cannot be empty");
            if (usernameToMatch == null)
                throw new ArgumentNullException("usernameToMatch cannot be null");
            if(usernameToMatch == "")
                throw new ArgumentException("usernameToMatch cannot be empty");

            if (!RoleExists(roleName))
                throw new ProviderException("Role name not found.");

            var users = db.Users
                .Where(x => x.UserName == usernameToMatch
                && 
                x.Collection.Contains(db.Roles.FirstOrDefault(y => y.RoleName == roleName)));
            return users.Select(x => x.FullName).ToArray();
        }

        public override string[] GetAllRoles()
        {
            return db.Roles.Select(x => x.RoleName).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
                throw new ProviderException("Username not found.");

            return user.Collection.Select(x => x.RoleName).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (roleName == "")
                throw new ArgumentException("Role name cannot be empty.");
            if (roleName == null)
                throw new ArgumentNullException("Role name cannot be null.");
            if (!RoleExists(roleName))
                throw new ProviderException("Role name not found.");
            return db.Users.Where(x => x.Collection.Where(y => y.RoleName == roleName).Count() > 0)
                .Select(z => z.UserName).ToArray();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (username == "" || roleName == "")
                throw new ArgumentException("Username or role name is empty.");
            if (username == null || roleName == null)
                throw new ArgumentNullException("Username or role name is null.");
            if (!RoleExists(roleName))
                throw new ProviderException("Role name not found.");
            var user = db.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
                throw new ProviderException("User not found");

            return user.Collection.FirstOrDefault(x => x.RoleName == roleName) == null ? false : true;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            if (roleNames.FirstOrDefault(x=> x == null) != null)
                throw new ArgumentNullException("Role name cannot be null.");
            if (roleNames.FirstOrDefault(x => x == "") != null)
                throw new ArgumentNullException("Role name cannot be empty.");

            if (roleNames.Where(x => !RoleExists(x) ? true : false).Count() > 0)
                throw new ProviderException("Role name not found.");

            if (usernames.Where(x => db.Users.FirstOrDefault(z => z.UserName == x) == null).Count() > 0)
            {
                throw new ProviderException("Username not found.");
            }
            if (roleNames.FirstOrDefault(x => x.Contains(",")) != null)
            {
                throw new ArgumentException("Role name contains \",\"");
            }
            if (usernames.FirstOrDefault(x => x.Contains(",")) != null)
            {
                throw new ArgumentException("Username contains \",\"");
            }

            var users = db.Users.Where(x => usernames.Contains(x.UserName));
            var roles = db.Roles.Where(x => roleNames.Contains(x.RoleName));

            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    role.Collection.Remove(user);
                    user.Collection.Remove(role);
                }
            }
            foreach (var user in users)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            }
            foreach (var role in roles)
            {
                db.Entry(role).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
        }

        public override bool RoleExists(string roleName)
        {
            return db.Roles.FirstOrDefault(x => x.RoleName == roleName) == null ? false : true;
        }
    }
}