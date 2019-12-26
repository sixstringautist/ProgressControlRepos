using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ProgressControl.WEB.util;
using ProgressControl.DAL.Entities;
using ProgressControl.DAL.EF;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.PostgreSql;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Dashboard;
using Hangfire.Processing;
using Hangfire.Storage;
using System.Configuration;
using ProgressControl.WEB_New_.util;

namespace ProgressControl.WEB_New_
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

    }
}
