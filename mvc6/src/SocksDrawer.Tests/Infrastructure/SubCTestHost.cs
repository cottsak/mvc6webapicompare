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
        LocalDb _localDb;

        public string ConnectionString { get; private set; }
        public TestServer Server { get; set; }

        public SubCTestHost()
        {
            _localDb = new LocalDb();
            ConnectionString = _localDb.OpenConnection().ConnectionString;
            Program.Main(new [] { ConnectionString });
            Server = Create.TestServer(ConnectionString);
        }

        public HttpClient CreateClient()
        {
            return Server.CreateClient();
        }

        public ISession GetSession()
        {
            return Fluently
                .Configure()
                .Database(() => MsSqlConfiguration.MsSql2012.ConnectionString(ConnectionString))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<SocksPair>())
                .BuildSessionFactory()
                .OpenSession();
        }

        public void Dispose()
        {
            _localDb.Dispose();
        }
    }
}
