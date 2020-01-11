using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ProgressControl.WEB.Models.Auth.Entities;

namespace ProgressControl.WEB.Models.Auth.EF
{
    public class UserDbInitializer : DropCreateDatabaseAlways<UserContext>
    {
        protected override void Seed(UserContext context)
        {
            Role admin = new Role() { RoleName = "admin"};
            Role user = new Role() { RoleName = "user" };
            var tmp = new User("","","", "CustomMembershipProvider", "admin",null
                ,"","","",true,DateTime.Now,
                DateTime.MinValue,DateTime.MinValue,
                DateTime.MinValue,DateTime.MinValue);
            var tmp1 = new User("", "", "", "CustomMembershipProvider", "Test", null
                , "", "", "", true, DateTime.Now,
                DateTime.MinValue, DateTime.MinValue,
                DateTime.MinValue, DateTime.MinValue);

            tmp.ChangePassword(null, "0000");
            tmp1.ChangePassword(null, "0000");
            user.Collection.Add(tmp1);
            tmp1.Collection.Add(user);
            admin.Collection.Add(tmp);
            tmp.Collection.Add(admin);
            context.Users.Add(tmp);
            context.Users.Add(tmp1);
            context.Roles.Add(admin);
            context.Roles.Add(user);
            context.SaveChanges();
            base.Seed(context);
        }
    }
}
