using System;
using System.Net.Http;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.AspNet.TestHost;
using NHibernate;
using Wco.Wocis.Migrations;
using WcoApi.Domain;

namespace WocApi.Tests.Infrastructure
{
    public class WocisDbSubcHost : IDisposable
    {
        LocalDb _localDb;

        public string ConnectionString { get; private set; }
        public TestServer Server { get; set; }

        public WocisDbSubcHost()
        {
            _localDb = new LocalDb();
            ConnectionString = _localDb.ConnectionString;
            Program.RunMigrations(new Options
            {
                ConnectionString = ConnectionString
            });
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
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<PolicyCancellationRequest>())
                .BuildSessionFactory()
                .OpenSession();
        }

        public void Dispose()
        {
            _localDb.Dispose();
        }
    }
}
