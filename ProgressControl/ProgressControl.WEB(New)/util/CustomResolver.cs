using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProgressControl.WEB.Models.Auth.EF;
using ProgressControl.DAL.EF;
using System.Configuration;
using ProgressControl.DAL.Entities;
namespace ProgressControl.WEB.util
{
    public class CustomResolver : IDependencyResolver, IDisposable
    {
        private readonly UserContext userContext;
        private RsContext rsContext;
        private DBF_Connector connector;

        public CustomResolver()
        {
            userContext = new UserContext(ConfigurationManager.ConnectionStrings["UsersConnection"].ConnectionString);
            rsContext = new RsContext(ConfigurationManager.ConnectionStrings["EFConnection"].ConnectionString);
            connector = new DBF_Connector(new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["dbfConnection"].ConnectionString), this.rsContext);
        }

        public void Dispose()
        {
            userContext.Dispose();
        }

        public object GetService(Type serviceType)
        {
            switch (serviceType.Name)
            {
                case "UserContext":
                    return userContext;
                    
                case "RsContext":
                    return rsContext;

                case "DBF_Connector":
                    return connector;
                default:
                    return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }
    }
}