using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Owin;
using System.Web.Mvc;
using ProgressControl.WEB.util;
using Hangfire;
using Hangfire.PostgreSql;
using System.Configuration;
using ProgressControl.DAL.Entities;
using ProgressControl.WEB_New_.util;
using Owin;

[assembly: OwinStartup(typeof(ProgressControl.WEB_New_.App_Start.Startup1))]

namespace ProgressControl.WEB_New_.App_Start
{
    public class Startup1
    {
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(ConfigurationManager.ConnectionStrings["HangfireConnection"].ConnectionString, new PostgreSqlStorageOptions
                {
                    InvisibilityTimeout = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DistributedLockTimeout = TimeSpan.FromMinutes(5),

                });

            yield return new BackgroundJobServer();
        }

        public void Configuration(IAppBuilder app)
        {
            DependencyResolver.SetResolver(new CustomResolver());
            GlobalConfiguration.Configuration.UseActivator(new CustomJobActivator(DependencyResolver.Current));
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard();
            //RecurringJob.AddOrUpdate<DBF_Connector>((x) => x.BackgroundTask(), Cron.Daily);
            BackgroundJob.Enqueue<DBF_Connector>((x) => x.BackgroundTask());


        }
    }
}
