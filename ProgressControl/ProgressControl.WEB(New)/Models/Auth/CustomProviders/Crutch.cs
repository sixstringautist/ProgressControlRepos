using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProgressControl.WEB.Models.Auth.EF;
using System.Web.Mvc;
using System.Configuration.Provider;
using ProgressControl.WEB.Models.Auth.Entities;
using System.Web.Security;
using PagedList;

namespace ProgressControl.WEB_New_.Models.Auth.CustomProviders
{
    public class Crutch : IDisposable
    {
        private UserContext db;

        public Crutch()
        {
            this.db = DependencyResolver.Current.GetService<UserContext>();
        }
        #region Users

        private void ThrowOnEmptyOrNull(params string[] parameters)
        {
            foreach (var el in parameters)
            {
                if (el == "")
                    throw new ArgumentException("Parameter cannot be empty");

                if (el == null)
                    throw new ArgumentNullException("Parameter cannot be null");
            }
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            ThrowOnEmptyOrNull(oldPassword, newPassword);

            var user = db.Users.FirstOrDefault(x => x.UserName == username);
            var res = user.ChangePassword(oldPassword, newPassword);
            user.LastActivityDate = DateTime.Now;
            if (res)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return res;
        }

        public  bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            ThrowOnEmptyOrNull(password, newPasswordQuestion, newPasswordAnswer);

            var user = db.Users.FirstOrDefault(x => x.UserName == username);
            bool res = user.ChangePasswordQuestionAndAnswer(password, newPasswordQuestion, newPasswordAnswer);
            user.LastActivityDate = DateTime.Now;
            if (res)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return res;
        }

        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ThrowOnEmptyOrNull(username, password);

            var user = new User("", "", "", this.ToString(), username, providerUserKey, email, passwordQuestion, "", isApproved, DateTime.Now, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
            if (db.Users.FirstOrDefault(x => x.UserName == user.UserName) == null)
            {
                user.LastActivityDate = DateTime.Now;
                user.ChangePassword(null, password);
                db.Users.Add(user);
                db.SaveChanges();
                status = MembershipCreateStatus.Success;
                return user;
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

        }

        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            ThrowOnEmptyOrNull(username);

            var user = db.Users.FirstOrDefault(x => x.UserName == username);

            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            ThrowOnEmptyOrNull(emailToMatch);
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be more than zero");

            totalRecords = db.Users.Count(x => x.Email == emailToMatch);
            var users = db.Users.ToPagedList(pageIndex, pageSize).Where(x => x.Email == emailToMatch);
            MembershipUserCollection collection = new MembershipUserCollection();
            foreach (var user in users)
                collection.Add(user);

            return collection;
        }

        public MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            ThrowOnEmptyOrNull(usernameToMatch);
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be more than zero");

            totalRecords = db.Users.Count(x => x.UserName == usernameToMatch);
            var users = db.Users.ToPagedList(pageIndex, pageSize).Where(x => x.UserName == usernameToMatch);
            MembershipUserCollection collection = new MembershipUserCollection();
            foreach (var user in users)
                collection.Add(user);

            return collection;
        }

        public MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be more than zero");
            totalRecords = db.Users.Count();
            var users = db.Users.ToPagedList(pageIndex, pageSize);
            MembershipUserCollection collection = new MembershipUserCollection();
            foreach (var user in users)
                collection.Add(user);
            return collection;
        }

        public int GetNumberOfUsersOnline()
        {
            int count = 0;
            foreach (var user in db.Users)
            {
                if (user.IsOnline)
                    count++;
            }
            return count;
        }

        public string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(string username, bool userIsOnline)
        {
            ThrowOnEmptyOrNull(username);
            var usr = db.Users.FirstOrDefault(x => x.UserName == username);
            if (userIsOnline)
            {
                usr.LastActivityDate = DateTime.Now;
            }
            return usr;
        }

        public string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public bool UnlockUser(string userName)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);

            if (user != null)
                return user.UnlockUser();
            else return false;
        }

        public void UpdateUser(MembershipUser user)
        {
            var usr = user as User;
            var tmp = db.Users.FirstOrDefault(x => x.UserName == usr.UserName);
            if (tmp == null)
                throw new ProviderException("User not exists");
            tmp.Collection = usr.Collection;
            tmp.Comment = usr.Comment;
            tmp.Email = usr.Email;
            tmp.FirstName = usr.FirstName;
            tmp.LastName = usr.LastName;
            tmp.Patronimyc = usr.Patronimyc;
            tmp.LastActivityDate = usr.LastActivityDate;
            tmp.LastLoginDate = usr.LastLoginDate;
            tmp.LastPasswordChangedDate = usr.LastPasswordChangedDate;
            tmp.IsApproved = usr.IsApproved;
            db.Entry(tmp).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public bool ValidateUser(string username, string password)
        {
            ThrowOnEmptyOrNull(username);
            var user = db.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
                return false;
            else
            {
                if (!user.IsLockedOut && user.Password == password && user.IsApproved)
                {
                    user.LastLoginDate = DateTime.Now;
                    user.LastActivityDate = DateTime.Now;
                    UpdateUser(user);
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Roles
        public void AddUsersToRoles(string[] usernames, string[] roleNames)
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

        public void CreateRole(string roleName)
        {
            if (roleName == null)
                throw new ArgumentNullException("Role name cannot be null");
            if (roleName == "")
                throw new ArgumentException("Role name cannot be empty");

            if (RoleExists(roleName))
                throw new ProviderException("Role name alredy exists.");

            var newRole = new Role() { RoleName = roleName };
            db.Roles.Add(newRole);
            db.SaveChanges();
        }

        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
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

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (roleName == null)
                throw new ArgumentNullException("Role name cannot be null");
            if (roleName == "")
                throw new ArgumentException("Role name cannot be empty");
            if (usernameToMatch == null)
                throw new ArgumentNullException("usernameToMatch cannot be null");
            if (usernameToMatch == "")
                throw new ArgumentException("usernameToMatch cannot be empty");

            if (!RoleExists(roleName))
                throw new ProviderException("Role name not found.");

            var users = db.Users
                .Where(x => x.UserName == usernameToMatch
                &&
                x.Collection.Contains(db.Roles.FirstOrDefault(y => y.RoleName == roleName)));
            return users.Select(x => x.FullName).ToArray();
        }

        public string[] GetAllRoles()
        {
            return db.Roles.Select(x => x.RoleName).ToArray();
        }

        public string[] GetRolesForUser(string username)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == username);
            if (user == null)
                throw new ProviderException("Username not found.");

            return user.Collection.Select(x => x.RoleName).ToArray();
        }

        public string[] GetUsersInRole(string roleName)
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

        public bool IsUserInRole(string username, string roleName)
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

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            if (roleNames.FirstOrDefault(x => x == null) != null)
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

        public bool RoleExists(string roleName)
        {
            return db.Roles.FirstOrDefault(x => x.RoleName == roleName) == null ? false : true;
        }
        #endregion
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}