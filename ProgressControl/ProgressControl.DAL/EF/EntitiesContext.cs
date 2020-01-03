using System;
using System.Data.Entity;
using ProgressControl.DAL.Entities;
namespace ProgressControl.DAL.EF
{
    public class RsContext : DbContext
    {
        public DbSet<Element> Elements { get; set; }

        public DbSet<Specification> Specifications { get; set; }

        public DbSet<Smt_box> SmtBoxes { get; set; }

        public DbSet<RsTask> GlobalTasks { get; set; }

        public DbSet<Subtask> SubTasks { get; set; }

        public DbSet<RsArea> RsAreas { get; set; }

        public DbSet<AreaTask> AreaTasks { get; set; }


        public RsContext(string connectionString): base(connectionString)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<RsContext>());
            Database.Initialize(true);
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
            modelBuilder.Entity<Element>().HasMany(x => x.Task).WithMany(x => x.TaskWarehouse);

            modelBuilder.Entity<RsTask>().ToTable("RsTasks");
            modelBuilder.Entity<Subtask>().ToTable("Subtasks");


            modelBuilder.Entity<AreaTask>().ToTable("AreaTasks");
            modelBuilder.Entity<AreaTask>().HasMany(x => x.TaskWarehouse).WithMany(x => x.Task);

            modelBuilder.Entity<RsArea>().HasKey(x => x.Code);

        }

    }

}
