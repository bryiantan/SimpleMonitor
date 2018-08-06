using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimpleMonitor.Web.Startup))]
namespace SimpleMonitor.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
