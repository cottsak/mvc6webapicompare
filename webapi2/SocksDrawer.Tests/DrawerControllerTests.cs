using System.Configuration;
using System.Linq;
using System.Net;
using Autofac;
using ControllerTests;
using NHibernate;
using NHibernate.Linq;
using Shouldly;
using SocksDrawer.MigrateDb;
using SocksDrawer.Models;
using Xunit;

namespace SocksDrawer.Tests
{
    public class DrawerControllerTests : ApiControllerTestBase<ISession>
    {
        static DrawerControllerTests()
        {
            // migrate db
            Program.Main(new[] { ConfigurationManager.ConnectionStrings["store"].ConnectionString });
        }

        public DrawerControllerTests() : base(new ApiTestSetup<ISession>(
            Startup.GetContainer(),
            WebApiConfig.Register,
            builder =>
            {
                // changing the ISession to a singleton so that the two ISession Resolve() calls
                // produce the same instance such that the transaction includes all test activity.
                builder.Register(context => NhibernateConfig.CreateSessionFactory().OpenSession())
                    .As<ISession>()
                    .SingleInstance();
            },
            session => session.BeginTransaction(),
            session => session.Transaction.Dispose(),   // tear down transaction to release locks
            session =>
            {
                NhibernateConfig.CompleteRequest(session);
                session.Clear();    // this is to ensure we don't get ghost results from the NHibernate cache  
            }))
        {

        }

        [Fact]
        public void WhenPostNewPairToDrawer_ThenResponseIsCreatedAndPairIsStored()
        {
            var response = Post("api/drawer", new { colour = "black" });

            // assert response is 201
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            // assert pair is in store
            var pairFromStore = Session.Query<SocksPair>().Single();
            pairFromStore.Id.ShouldNotBe(0);
            pairFromStore.Colour.ShouldBe(SocksColour.Black);
        }
    }
}
