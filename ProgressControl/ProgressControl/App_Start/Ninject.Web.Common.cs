[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ProgressControl.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(ProgressControl.App_Start.NinjectWebCommon), "Stop")]

namespace ProgressControl.App_Start
{
    using System;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using ProgressControl.DAL.Repositories;
    using ProgressControl.DAL.Interfaces;
    using ProgressControl.DAL.Entities;
    using ProgressControl.DAL.EF;
    using ProgressControl.DAL.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Owin.Security;
    using System.Configuration;

    public static class NinjectWebCommon 
    {
        public static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel.Bind<ApplicationContext>().ToSelf()
                .InRequestScope()
                .WithConstructorArgument(ConfigurationManager.ConnectionStrings["EFConnection"].ConnectionString);

            kernel.Bind<IUserStore<ApplicationUser>>()
                .To<UserStore<ApplicationUser>>().InRequestScope()
                .WithConstructorArgument(kernel.Get<ApplicationContext>());

            kernel.Bind<IAuthenticationManager>()
                .ToMethod(c => HttpContext.Current.GetOwinContext().Authentication)
                .InRequestScope();

            kernel.Bind<ApplicationUserManager>().ToSelf()
                .InRequestScope().WithConstructorArgument(kernel.Get<ApplicationContext>());

            kernel.Bind<ApplicationRoleManager>().ToSelf()
                .InRequestScope()
                .WithConstructorArgument(kernel.Get<ApplicationContext>());


            kernel.Bind<ApplicationSignInManager>().ToSelf().InRequestScope()
                .WithConstructorArgument(kernel.Get<ApplicationUserManager>())
                .WithConstructorArgument(kernel.Get<IAuthenticationManager>());

            kernel.Bind<IUnitOfWork>().To<UnitOfWork>()
                .InRequestScope().WithConstructorArgument(kernel.Get<ApplicationContext>());
        }        
    }
}