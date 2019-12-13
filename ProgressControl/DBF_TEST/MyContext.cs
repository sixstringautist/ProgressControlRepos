using System.Data.Entity;
using System;
namespace DBF_TEST
{
    public class MyContext : DbContext,IDisposable
    {
        public DbSet<Element> Elements { get; set; }

        public DbSet<Specification> Specifications { get; set; }

        public MyContext(): base("EFConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<MyContext>());
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Analog>()
                .HasKey(u => new { u.Code, u.CodeTwo });
            modelBuilder.Entity<Analog>().HasRequired(u => u.NavProp).WithMany(u => u.Parents)
                .HasForeignKey(u => u.Code);
            modelBuilder.Entity<Analog>().HasRequired(u => u.NavPropTwo).WithMany(u => u.Childrens)
                .HasForeignKey(u => u.CodeTwo);
            modelBuilder.Entity<Element>().HasMany(u => u.Childrens).WithRequired(u => u.NavPropTwo);
            modelBuilder.Entity<Element>().HasMany(u => u.Parents).WithRequired(u=> u.NavProp);
        }
    }

}
