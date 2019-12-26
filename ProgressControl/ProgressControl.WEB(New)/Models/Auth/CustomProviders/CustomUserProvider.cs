using System;
using System.Linq;
using System.Web.Security;
using ProgressControl.WEB.Models.Auth.EF;
using ProgressControl.WEB.Models.Auth.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Configuration.Provider;
using PagedList;
namespace ProgressControl.WEB.Models.Auth.CustomProviders
{
    public class CustomUserProvider : MembershipProvider
    {
        UserContext db;
        private string _applicationName;

        private  bool _enablePasswordRetrieval;

        private  bool _enablePasswordReset;

        private  bool _requiresQuestionAndAnswer;

        private  int _maxInvalidPasswordAttempts;

        private  int _passwordAttemptWindow;

        private  bool _requiresUniqueEmail;

        private  MembershipPasswordFormat _passwordFormat;

        private  int _minRequiredPasswordLength;

        private  int _minRequiredNonAlphanumericCharacters;

        private  string _passwordStrengthRegularExpression;

        //TODO: Implement Provider Methods
        public override bool EnablePasswordRetrieval => _enablePasswordRetrieval;

        public override bool EnablePasswordReset => _enablePasswordReset;

        public override bool RequiresQuestionAndAnswer => _requiresQuestionAndAnswer;

        public override string ApplicationName { get => _applicationName; set => _applicationName = value; }

        public override int MaxInvalidPasswordAttempts => _maxInvalidPasswordAttempts;

        public override int PasswordAttemptWindow => _passwordAttemptWindow;

        public override bool RequiresUniqueEmail => _requiresUniqueEmail;

        public override MembershipPasswordFormat PasswordFormat => _passwordFormat;

        public override int MinRequiredPasswordLength => _minRequiredPasswordLength;

        public override int MinRequiredNonAlphanumericCharacters => _minRequiredNonAlphanumericCharacters;

        public override string PasswordStrengthRegularExpression => _passwordStrengthRegularExpression;

        static byte[] GetPasswordHash(string password)
        {
            byte[] bytes;
            using (SHA256 sha256hash = SHA256.Create())
            {
                bytes = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            return bytes;
        }
        public CustomUserProvider() : base()
        {
            this.db = DependencyResolver.Current.GetService<UserContext>();
        }

        public CustomUserProvider(UserContext db)
        {
            this.db = db;
        }

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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
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

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
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

         public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ThrowOnEmptyOrNull(username,password);

            var user = new User("","","",this.ToString(),username, providerUserKey, email,passwordQuestion,"", isApproved,DateTime.Now,DateTime.MinValue,DateTime.MinValue,DateTime.MinValue,DateTime.MinValue);
            if (db.Users.FirstOrDefault(x => x.UserName == user.UserName) == null)
            {
                user.LastActivityDate = DateTime.Now;
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

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
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

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            ThrowOnEmptyOrNull(emailToMatch);
            if (pageSize <= 0)
                throw new ArgumentException("Page size must be more than zero");

            totalRecords = db.Users.Count(x => x.Email == emailToMatch);
            var users = db.Users.ToPagedList(pageIndex, pageSize).Where(x=> x.Email == emailToMatch);
            MembershipUserCollection collection = new MembershipUserCollection();
            foreach (var user in users)
                collection.Add(user);

            return collection;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
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

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
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

        public override int GetNumberOfUsersOnline()
        {
            int count = 0;
            foreach (var user in db.Users)
            {
                if (user.IsOnline)
                    count++;
            }
            return count;
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            ThrowOnEmptyOrNull(username);
            var usr =  db.Users.FirstOrDefault(x => x.UserName == username);
            if (userIsOnline)
            {
                usr.LastActivityDate = DateTime.Now;
            }
            return usr;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);

            if (user != null)
                return user.UnlockUser();
            else return false;
        }

        public override void UpdateUser(MembershipUser user)
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

        public override bool ValidateUser(string username, string password)
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

    }
}