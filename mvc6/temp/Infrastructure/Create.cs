using System.Collections.Generic;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WcoApi;

class Create
{
    public static TestServer TestServer(string connectionString)
    {
        var startup = new Startup(new KeyValuePair<string, string>("ConnectionString", connectionString));
        return Microsoft.AspNet.TestHost.TestServer.Create(app =>
        {
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            startup.Configure(app, env, loggerFactory);
        }, services => startup.ConfigureServices(services));
    }
}