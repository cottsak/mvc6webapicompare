using System;
using System.Net.Http;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNet.TestHost;
using NHibernate;
using SocksDrawer.MigrateDb;
using SocksDrawer.Mvc6.Models;

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
            Server = Create.TestServer(connectionString);

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
}
