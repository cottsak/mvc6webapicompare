using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using SocksDrawer.Mvc6.Models;

namespace SocksDrawer.Mvc6
{
    class NhibernateConfig
    {
        internal static ISessionFactory CreateSessionFactory(string connectionString)
        {
            return Fluently
                .Configure()
                .Database(() =>
                    MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
                .Mappings(mc => mc.FluentMappings.AddFromAssemblyOf<SocksPair>())
                .BuildSessionFactory();
        }

        internal static Action<ISession> CompleteRequest = session =>
        {
            if (session.IsDirty())
                session.Flush();        // deletes won't work without explicit .Flush()
        };
    }
}
