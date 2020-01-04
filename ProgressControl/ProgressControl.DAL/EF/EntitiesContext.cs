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
            Database.Initialize(false);
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
            modelBuilder.Entity<Element>().Map(x =>
            {
                x.MapInheritedProperties();
                x.ToTable("Elements");
            });

            modelBuilder.Entity<Smt_box>().Map(x =>
            {
                x.MapInheritedProperties();
                x.ToTable("SmtBoxes");
            });

            modelBuilder.Entity<Smt_box>().HasOptional(x => x.Container).WithMany(x=> x.Elements);

            modelBuilder.Entity<RsTask>().ToTable("RsTasks");
            modelBuilder.Entity<RsTask>().HasMany(x => x.Subtasks).WithRequired(x => x.NavProp);
            

            modelBuilder.Entity<Subtask>().ToTable("Subtasks");
            modelBuilder.Entity<Subtask>().HasMany(x => x.AreaTasks).WithRequired(x => x.Subtask);
            modelBuilder.Entity<Subtask>().HasRequired(x => x.NavProp).WithMany(x => x.Subtasks);
            modelBuilder.Entity<Subtask>().HasRequired(x => x.Specification);


            modelBuilder.Entity<AreaTask>().ToTable("AreaTasks");
            modelBuilder.Entity<AreaTask>().HasRequired(x => x.Subtask).WithMany(x => x.AreaTasks);
            modelBuilder.Entity<AreaTask>().HasRequired(x => x.Container).WithRequiredPrincipal(x => x.NavProp);


            modelBuilder.Entity<Container>().HasRequired(x => x.NavProp).WithRequiredDependent(x=> x.Container);
            modelBuilder.Entity<Container>().HasMany(x => x.Elements).WithOptional(x=> x.Container);

            modelBuilder.Entity<WarehouseArea>().HasMany(x => x.WarehouseTasks).WithRequired(x => x.WarehouseArea);
            modelBuilder.Entity<SmtLineArea>().HasMany(x => x.SmtLineTasks).WithRequired(x => x.SmtLineArea);


            modelBuilder.Entity<WarehouseTask>().HasRequired(x => x.WarehouseArea).WithMany(x => x.WarehouseTasks);
            modelBuilder.Entity<SmtLineTask>().HasRequired(x => x.SmtLineArea).WithMany(x => x.SmtLineTasks);

            modelBuilder.Entity<RsArea>().HasKey(x => x.Code);

        }

    }

}
