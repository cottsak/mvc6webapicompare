using System.Collections.Generic;
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
            session => session.Transaction.Dispose(), // tear down transaction to release locks
            session =>
            {
                NhibernateConfig.CompleteRequest(session);
                session.Clear(); // this is to ensure we don't get ghost results from the NHibernate cache  
            }))
        { }

        [Fact]
        public void WhenPostNewPairToDrawer_ThenResponseIsCreatedAndPairIsOnlyOneInStore()
        {
            var response = Post("api/drawer", new { colour = "black" });

            // assert response is 201
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            // assert pair is in store by itself
            var pairFromStore = Session.Query<SocksPair>().Single();
            pairFromStore.Id.ShouldNotBe(0);
            pairFromStore.Colour.ShouldBe(SocksColour.Black);
        }

        [Fact]
        public void GivenTwoBlackPairsInStore_WhenGetAllPairs_ThenTwoAreReturned()
        {
            var twoPairs = new[] { new SocksPair(SocksColour.Black), new SocksPair(SocksColour.Black), };
            twoPairs.ToList().ForEach(p => Session.Save(p));
            Session.Flush();

            var pairs = Get("api/drawer/socks").BodyAs<IEnumerable<SocksPair>>();

            pairs.Count().ShouldBe(2);
            pairs.ShouldAllBe(p => p.Colour == SocksColour.Black);
        }

        [Fact]
        public void GivenTwoBlackPairsInStore_WhenDeleteOne_ThenOnlyOneRemains()
        {
            Session.Save(new SocksPair(SocksColour.Black));
            var pairTwo = new SocksPair(SocksColour.Black);
            Session.Save(pairTwo);
            Session.Flush();

            var response = Delete($"/api/drawer/{pairTwo.Id}");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            Session.Query<SocksPair>().Count().ShouldBe(1);
            Session.Query<SocksPair>().SingleOrDefault(p => p.Id == pairTwo.Id).ShouldBeNull();
        }

        // get one

        // assert max white rule
    }
}
