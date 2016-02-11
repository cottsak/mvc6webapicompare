using System.Collections.Generic;
using System.Linq;
using System.Net;
using ControllerTests;
using NHibernate;
using NHibernate.Linq;
using Shouldly;
using SocksDrawer.Mvc6;
using SocksDrawer.Mvc6.Models;
using Xunit;
using Autofac;

namespace SocksDrawer.Tests
{
    public class DrawerControllerTests : ApiControllerTestBase<ISession>
    {
        public DrawerControllerTests() : base(new ApiTestSetup<ISession>(
            Startup.GetContainer(null),     // shouldn't cause a NRE because we're replacing the ISession rego so the delegate should never be invoked
            configuration => { },
            builder =>
                {
                    builder.RegisterType<LocalDb>().AsSelf();   // this needs to be registered so it's Disposed properly

                    // changing the ISession to a singleton so that the two ISession Resolve() calls
                    // produce the same instance such that the transaction includes all test activity.
                    builder.Register(context =>
                    {
                        var connString = context.Resolve<LocalDb>().OpenConnection().ConnectionString;
                        // migrate empty db
                        //Program.Main(new[] { connString });

                        return NhibernateConfig.CreateSessionFactory(connString).OpenSession();
                    })
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

        //[Fact]
        //public void WhenPostNewPairToDrawer_ThenResponseIsCreatedAndPairIsOnlyOneInStore()
        //{
        //    var response = Post("api/drawer", new { colour = "black" });

        //    // assert response is 201
        //    response.StatusCode.ShouldBe(HttpStatusCode.Created);
        //    // assert pair is in store by itself
        //    var pairFromStore = Session.Query<SocksPair>().Single();
        //    pairFromStore.Id.ShouldNotBe(0);
        //    pairFromStore.Colour.ShouldBe(SocksColour.Black);
        //}

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
    }
}
