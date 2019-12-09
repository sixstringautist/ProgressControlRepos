using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProgressControl.Startup))]
namespace ProgressControl
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
