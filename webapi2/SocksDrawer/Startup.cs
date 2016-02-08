using System.Web.Http;
using Microsoft.Owin;
using Owin;
using SocksDrawer;

[assembly: OwinStartup(typeof(Startup))]

namespace SocksDrawer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
