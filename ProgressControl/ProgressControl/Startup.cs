using Microsoft.Owin;
using System.Diagnostics;
using Owin;
using Hangfire;
using System;
using System.Collections.Generic;
using Hangfire.SqlServer;
using DBF_TEST;

[assembly: OwinStartupAttribute(typeof(ProgressControl.Startup))]
namespace ProgressControl
{
    public partial class Startup
    {

        private IEnumerable<IDisposable> GetHangfireServers()
        {
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=H:\sources\ProgressControlRepos\ProgressControl\ProgressControl\App_Data\HangfireTest.mdf;Integrated Security=True", new SqlServerStorageOptions
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

        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard();
            ConfigureAuth(app);
        }
    }
}
