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
    public class CustomResolver : IDependencyResolver
    {

        public CustomResolver()
        {

        }


        public object GetService(Type serviceType)
        {
            switch (serviceType.Name)
            {
                case "UserContext":
                    return Activator.CreateInstance(serviceType, ConfigurationManager.ConnectionStrings["UsersConnection"].ConnectionString);
                    
                case "RsContext":
                    return Activator.CreateInstance(serviceType, ConfigurationManager.ConnectionStrings["EFConnection"].ConnectionString);

                case "DBF_Connector":
                    return Activator.CreateInstance(serviceType, new System.Data.OleDb.OleDbConnection(ConfigurationManager.ConnectionStrings["dbfConnection"].ConnectionString), 
                        GetService(typeof(RsContext)));
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