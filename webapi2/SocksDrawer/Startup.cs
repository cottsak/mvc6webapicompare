using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using SocksDrawer;
using SocksDrawer.Controllers;
using SocksDrawer.Models;

[assembly: OwinStartup(typeof(Startup))]

namespace SocksDrawer
{
    public class Startup
    {
        internal static IContainer GetContainer()
        {
            var builder = new ContainerBuilder();

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Run other optional steps, like registering filters,
            // per-controller-type services, etc., then set the dependency resolver
            // to be Autofac.
            builder.RegisterType<ValuesGetter>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<SocksDrawerRepository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            //var container = builder.Build();
            return builder.Build();
        }

        public void Configuration(IAppBuilder app)
        {
            // STANDARD WEB API SETUP:

            // Get your HttpConfiguration. In OWIN, you'll create one
            // rather than using GlobalConfiguration.
            var config = new HttpConfiguration();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(GetContainer());

            // OWIN WEB API SETUP:

            // Register the Autofac middleware FIRST, then the Autofac Web API middleware,
            // and finally the standard Web API middleware.
            app.UseAutofacMiddleware(GetContainer());
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);

            WebApiConfig.Register(config);
        }
    }
}
