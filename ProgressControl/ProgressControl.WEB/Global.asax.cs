using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Hangfire;
using Hangfire.SqlServer;
using ProgressControl.WEB.util;
using ProgressControl.DAL.Entities;
using ProgressControl.DAL.EF;

namespace ProgressControl.WEB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=aspnet-MvcMovie;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|\HangfireTest.mdf", new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                });

            yield return new BackgroundJobServer();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DependencyResolver.SetResolver(new CustomResolver());
            var context = new RsContext("Host=localhost;Port=5432;Username=postgres;Password=root;Database=SMT_Updating_Repository");
            DBF_Connector conn = new DBF_Connector(new System.Data.OleDb.OleDbConnection(@"Provider=VFPOLEDB.1;Data Source=H:\ДИПЛОМНАЯ РАБОТА\SMT;User ID=admin"), context);

            HangfireAspNet.Use(GetHangfireServers);

            conn.BackgroundTask();
            context.Elements.FirstOrDefault(x => x.Code == 8);
        }
    }
}
