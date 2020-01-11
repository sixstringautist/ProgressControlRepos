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

        public DbSet<WarehouseTask> WarehouseTasks { get; set; }

        public DbSet<SmtLineTask> SmtLineTasks { get; set; }


        public RsContext(string connectionString): base(connectionString)
        {
            Database.SetInitializer(new RsInitializator());
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

            modelBuilder.Entity<Element>().HasKey(x => x.Code);
            modelBuilder.Entity<Element>().Property(x => x.Code).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            modelBuilder.Entity<Smt_box>().HasKey(x => x.Code);
            modelBuilder.Entity<Smt_box>().Property(x => x.Code).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Smt_box>().HasRequired(x => x.Container).WithMany(x=> x.Elements).HasForeignKey(x=> x.ContainerId);
            modelBuilder.Entity<Smt_box>().HasMany(x => x.HistoryPoints).WithRequired(x => x.NavProp);

            modelBuilder.Entity<BoxHistory>().HasRequired(x => x.NavProp).WithMany(x => x.HistoryPoints);
            modelBuilder.Entity<BoxHistory>().HasKey(x => x.Code);

            modelBuilder.Entity<RsTask>().ToTable("RsTasks");
            modelBuilder.Entity<RsTask>().HasMany(x => x.Subtasks).WithRequired(x => x.NavProp);
            

            modelBuilder.Entity<Subtask>().ToTable("Subtasks");
            modelBuilder.Entity<Subtask>().HasMany(x => x.AreaTasks).WithRequired(x => x.Subtask);
            modelBuilder.Entity<Subtask>().HasRequired(x => x.NavProp).WithMany(x => x.Subtasks);
            modelBuilder.Entity<Subtask>().HasRequired(x => x.Specification);

            modelBuilder.Entity<AreaTask>().ToTable("AreaTasks");
            modelBuilder.Entity<AreaTask>().HasRequired(x => x.Subtask).WithMany(x => x.AreaTasks).HasForeignKey(x=> x.SubtaskId);
            modelBuilder.Entity<AreaTask>().HasRequired(x => x.Container).WithMany(x=> x.Collection).HasForeignKey(x=> x.ContainerId);


            modelBuilder.Entity<Container>().HasMany(x => x.Collection).WithRequired(x => x.Container);
            modelBuilder.Entity<Container>().HasMany(x => x.Elements).WithRequired(x=> x.Container);
            modelBuilder.Entity<Container>().Property(x => x.Code).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<WarehouseArea>().HasMany(x => x.WarehouseTasks).WithRequired(x => x.Area).HasForeignKey(x=> x.AreaId);
            modelBuilder.Entity<SmtLineArea>().HasMany(x => x.SmtLineTasks).WithRequired(x => x.Area).HasForeignKey(x=> x.AreaId);

            modelBuilder.Entity<WarehouseTask>().HasRequired(x => x.Area).WithMany(x => x.WarehouseTasks);
            modelBuilder.Entity<SmtLineTask>().HasRequired(x => x.Area).WithMany(x => x.SmtLineTasks);

            modelBuilder.Entity<RsArea>().HasKey(x => x.Code);


        }

    }

}
