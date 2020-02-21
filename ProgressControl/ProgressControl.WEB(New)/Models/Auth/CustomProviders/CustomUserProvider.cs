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
using ProgressControl.WEB_New_.Models.Auth.CustomProviders;
namespace ProgressControl.WEB.Models.Auth.CustomProviders
{
    public class CustomUserProvider : MembershipProvider
    {
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

        }



        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            using (var tmp = new Helper())
            {
                return tmp.ChangePassword(username, oldPassword, newPassword);
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            using (var tmp = new Helper())
            {
                return tmp.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
            }
        }

         public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            using (var tmp = new Helper())
            {
                return tmp.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
            }
            
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (var tmp = new Helper())
            {
                return tmp.DeleteUser(username, deleteAllRelatedData);
            }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            using (var tmp = new Helper())
            {
               return tmp.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
            }
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            using (var tmp = new Helper())
            {
                return tmp.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
            }
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            using (var tmp = new Helper())
            {
                return tmp.GetAllUsers(pageIndex, pageIndex, out totalRecords);
            }
        }

        public override int GetNumberOfUsersOnline()
        {
            using (var tmp = new Helper())
            {
                return tmp.GetNumberOfUsersOnline();
            }
        }

        public override string GetPassword(string username, string answer)
        {
            using (var tmp = new Helper())
            {
                return tmp.GetPassword(username,answer);
            }
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (var tmp = new Helper())
            {
                return tmp.GetUser(providerUserKey,userIsOnline);
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (var tmp = new Helper())
            {
                return tmp.GetUser(username,userIsOnline);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            using (var tmp = new Helper())
            {
                return tmp.GetUserNameByEmail(email);
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            using (var tmp = new Helper())
            {
                return tmp.ResetPassword(username,answer);
            }
        }

        public override bool UnlockUser(string userName)
        {
            using (var tmp = new Helper())
            {
                return tmp.UnlockUser(userName);
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            using (var tmp = new Helper())
            {
                tmp.UpdateUser(user);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            using (var tmp = new Helper())
            {
                return tmp.ValidateUser(username, password);
            }
        }

    }
}