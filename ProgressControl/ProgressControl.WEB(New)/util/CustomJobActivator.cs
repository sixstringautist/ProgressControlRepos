using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire;
using System.Web.Mvc;
namespace ProgressControl.WEB_New_.util
{
    public class CustomJobActivator : JobActivator
    {
        private IDependencyResolver resolver;

        public CustomJobActivator(IDependencyResolver resolver)
        {
            this.resolver = resolver;
        }
        public override object ActivateJob(Type jobType)
        {
            return resolver.GetService(jobType);
        }
    }
}