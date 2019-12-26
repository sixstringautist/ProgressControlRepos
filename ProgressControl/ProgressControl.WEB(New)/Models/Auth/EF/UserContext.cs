using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ProgressControl.WEB.Models.Auth.Entities;

namespace ProgressControl.WEB.Models.Auth.EF
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<ControllerName> Controllers { get; set; }

        public DbSet<ActionName> Actions { get; set; }

        public UserContext(string connectionString):base(connectionString)
        {
            Database.SetInitializer(new UserDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(User.GetConfiguration());

            modelBuilder.Entity<Role>().HasKey(x => x.Code);
            modelBuilder.Entity<Role>().HasMany(x => x.Collection).WithMany(x => x.Collection);
        }
    }
}
