using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProgressControl.WEB.Models.Auth.EF;
using ProgressControl.DAL.EF;
using System.Configuration;
namespace ProgressControl.WEB.util
{
    public class CustomResolver : IDependencyResolver, IDisposable
    {
        private readonly UserContext userContext;
        private RsContext rsContext { get; set; }

        public CustomResolver()
        {
            userContext = new UserContext(ConfigurationManager.ConnectionStrings["UsersConnection"].ConnectionString);
            rsContext = new RsContext(ConfigurationManager.ConnectionStrings["EFConnection"].ConnectionString);
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