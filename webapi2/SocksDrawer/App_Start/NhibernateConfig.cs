using System;
using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using SocksDrawer.Models;

namespace SocksDrawer
{
    class NhibernateConfig
    {
        internal static ISessionFactory CreateSessionFactory()
        {
            return Fluently
                .Configure()
                .Database(() =>
                    MsSqlConfiguration.MsSql2012.ConnectionString(ConfigurationManager.ConnectionStrings["store"].ConnectionString))
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