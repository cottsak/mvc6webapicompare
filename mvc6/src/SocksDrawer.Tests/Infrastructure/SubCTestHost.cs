using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NHibernate;
using SocksDrawer.Mvc6;
using SocksDrawer.Mvc6.Models;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using Program = SocksDrawer.MigrateDb.Program;

namespace SocksDrawer.Tests.Infrastructure
{
    public class SubCTestHost : IDisposable
    {
        readonly LocalDb _localDb;
        private readonly Lazy<ISession> _session = null;

        public TestServer Server { get; set; }

        public SubCTestHost()
        {
            _localDb = new LocalDb();
            var connectionString = _localDb.OpenConnection().ConnectionString;
            Program.Main(new[] { connectionString });
            Server = CreateTestServer(connectionString);

            _session = new Lazy<ISession>(() =>
            {
                return Fluently
                    .Configure()
                    .Database(() => MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<SocksPair>())
                    .BuildSessionFactory()
                    .OpenSession();
            });
        }

        public static TestServer CreateTestServer(string connectionString)
        {
            var startup = new Startup(new KeyValuePair<string, string>("ConnectionString", connectionString));
            return TestServer.Create(app =>
            {
                var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
                var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

                startup.Configure(app, env, loggerFactory);
            }, services => startup.ConfigureServices(services));
        }

        public HttpClient CreateClient()
        {
            return Server.CreateClient();
        }

        public ISession Session => _session.Value;

        public void Dispose()
        {
            _localDb.Dispose();
        }
    }

    public static class TestExtensions
    {
        public static async Task<HttpResponseMessage> PostJsonAsync(this HttpClient client, string requestUri, object content)
        {
            var postContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return await client.PostAsync(requestUri, postContent);
        }

        public static T BodyAs<T>(this HttpResponseMessage response)
        {
            if (typeof(T) == typeof(string))
                return (T)(object)response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }
    }
}
