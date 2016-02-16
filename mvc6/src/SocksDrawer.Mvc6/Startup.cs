using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace SocksDrawer.Mvc6
{
    public class Startup
    {
        public static IContainer GetContainer(IConfigurationRoot configuration, Action<ContainerBuilder> builderActions = null)
        {
            var builder = new ContainerBuilder();

            // register dependencies
            builder.Register(context => NhibernateConfig.CreateSessionFactory(configuration["ConnectionString"]).OpenSession())
                .As<ISession>()
                .InstancePerLifetimeScope()
                .OnRelease(session =>
                {
                    NhibernateConfig.CompleteRequest(session);

                    session.Dispose();
                });

            builderActions?.Invoke(builder);

            return builder.Build();
        }

        public Startup() : this(new KeyValuePair<string, string>[0]) { }

        public Startup(params KeyValuePair<string, string>[] configOverrides)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddInMemoryCollection(configOverrides);
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var container = GetContainer(Configuration, builder => builder.Populate(services));
            return container.ResolveOptional<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
