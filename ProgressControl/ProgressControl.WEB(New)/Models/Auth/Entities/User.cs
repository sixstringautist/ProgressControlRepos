using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgressControl.DAL.Entities;
using System.Web.Security;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;

namespace ProgressControl.WEB.Models.Auth.Entities
{
    public class User : MembershipUser
    {

        private int id;
        private DateTime _creationDate;
        private DateTime _lastLockoutDate;
        private DateTime _lastPasswordChangedDate;
        private string _providerName;
        private string _userName;
        private string _passwordQuestion;
        private string password;
        private string passwordAnswer;


        [Key]
        public int Id { get => id; private set => id = value; }
        public override object ProviderUserKey => id;

        public override bool IsApproved { get; set; }
        public override bool IsLockedOut {
            get
            {
                return (DateTime.Now - _lastLockoutDate).Minutes < Membership.PasswordAttemptWindow;
            }
        }
        public override bool IsOnline
        {
            get
            {
                return (DateTime.Now - LastActivityDate).Minutes < Membership.UserIsOnlineTimeWindow;
            }
        }

        new public DateTime CreationDate { get => _creationDate; private set => _creationDate = value; }
        public override DateTime LastActivityDate { get; set; }
        new public DateTime LastLockoutDate { get => _lastLockoutDate; private set => _lastLockoutDate = value; }
        public override DateTime LastLoginDate { get; set; }
        new public DateTime LastPasswordChangedDate { get => _lastPasswordChangedDate; set => _lastPasswordChangedDate = value; }

        new public string ProviderName { get => _providerName; private set => _providerName = value; }
        new public string PasswordQuestion { get => _passwordQuestion; private set => _passwordQuestion = value; }
        public string Password { get => password; private set => password = value; }
        new public string UserName { get => _userName; private set => _userName = value; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronimyc { get; set; }
        public string FullName => string.Join(" ", LastName, FirstName, Patronimyc);

        public virtual ICollection<Role> Collection { get; set; }

        public User(string firstname, string lastname, string patronimyc,
            string providerName, string name,
            object providerUserKey, string email,
            string passwordQuestion, string comment,
            bool isApproved,
            DateTime creationDate, DateTime lastLoginDate,
            DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate)
        {
            FirstName = firstname;
            LastName = lastname;
            Patronimyc = patronimyc;
            this._providerName = providerName;
            this._userName = name;
            this.Email = email;
            this.PasswordQuestion = passwordQuestion;
            this.Comment = comment;
            this.IsApproved = isApproved;
            this.CreationDate = DateTime.Now;
            this.LastLoginDate = LastLoginDate;
            this.LastActivityDate = lastActivityDate;
            this.LastPasswordChangedDate = LastPasswordChangedDate;
            this._lastLockoutDate = lastLockoutDate;
            Collection = new List<Role>();
        }




        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            if (oldPassword == password)
            {
                password = newPassword;
                _lastPasswordChangedDate = DateTime.Now;
                return true;
            }
            else return false;
        }
        public override bool ChangePasswordQuestionAndAnswer(string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            if (this.password == password)
            {
                _passwordQuestion = newPasswordQuestion;
                passwordAnswer = newPasswordAnswer;
                return true;
            }
            else return false;
        }
      
        public override string GetPassword()
        {
            return Password;
        }
        public override string ResetPassword()
        {
            List<char> valid = new List<char>();
            for (char i = '0'; i < '9'; i++)
            {
                valid.Append(i);
            }
            for (char i = 'A'; i < 'Z'; i++)
            {
                valid.Append(i);
            }
            var rand = new Random();
            for (int i = 0; i < valid.Count; i++)
            {
                var rIndex = rand.Next(0, valid.Count);
                var rindex = rand.Next(0, valid.Count);
                char tmp = valid[rIndex];
                valid[rIndex] = valid[rindex];
                valid[rindex] = tmp;
            }
            return new string(valid.ToArray());
        }
        public override string GetPassword(string passwordAnswer)
        {
            throw new NotImplementedException();
        }
        public override string ResetPassword(string passwordAnswer)
        {
            throw new NotImplementedException();
        }
        public User() : base()
        {
            Collection = new List<Role>();
        }

        public override bool UnlockUser()
        {
            if (IsLockedOut)
            {
                this.LastLockoutDate = DateTime.MinValue;
                return true;
            }
            else return false;
        }
        public static UserConfiguration GetConfiguration()
        {
            return new UserConfiguration();
        }
        public class UserConfiguration : EntityTypeConfiguration<User>
        {
            public UserConfiguration()
            {
                HasMany(x => x.Collection).WithMany(x => x.Collection);
            }
        }
    }
}
