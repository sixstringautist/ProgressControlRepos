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
                ,"","","",true,false,DateTime.Now,
                DateTime.MinValue,DateTime.MinValue,
                DateTime.MinValue,DateTime.MinValue);
            tmp.ChangePassword(null, "0000");
            admin.Collection.Add(tmp);
            tmp.Collection.Add(admin);
            context.Users.Add(tmp);
            context.Roles.Add(admin);
            context.Roles.Add(user);

            context.SaveChanges();
            base.Seed(context);
        }
    }
}
