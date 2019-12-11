using System.Data.Entity;
namespace DBF_TEST
{
    public class MyContext : DbContext
    {
        public DbSet<Element> Elements { get; set; }

        public DbSet<Specification> Specifications { get; set; }



        public MyContext(): base("defaultConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<MyContext>());
        }
    }
}
