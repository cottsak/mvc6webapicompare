using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NHibernate.Linq;
using Shouldly;
using SocksDrawer.Mvc6.Models;
using SocksDrawer.Tests.Infrastructure;
using Xunit;

namespace SocksDrawer.Tests
{
    public class DrawerControllerTests
    {
        [Fact]
        public async Task WhenPostNewPairToDrawer_ThenResponseIsCreatedAndPairIsOnlyOneInStore()
        {
            using (var host = new SubCTestHost())
            {
                var response = await host.CreateClient().PostJsonAsync("api/drawer", new {colour = "black"});

                // assert response is 201
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                response.Headers.Location.ToString().ShouldContain("api/drawer");
                // assert pair is in store by itself
                var pairFromStore = host.GetSession().Query<SocksPair>().Single();
                pairFromStore.Id.ShouldNotBe(0);
                pairFromStore.Colour.ShouldBe(SocksColour.Black);
            }
        }
    }

    public class AnotherTestCollection      // only doing this for parallel xunit tests
    {
        [Fact]
        public async Task GivenTwoBlackPairsInStore_WhenGetAllPairs_ThenTwoAreReturned()
        {
            using (var host = new SubCTestHost())
            {
                var session = host.GetSession();
                var twoPairs = new[] { new SocksPair(SocksColour.Black), new SocksPair(SocksColour.Black), };
                twoPairs.ToList().ForEach(p => session.Save(p));
                session.Flush();

                var pairs = (await host.CreateClient().GetAsync("api/drawer/socks")).BodyAs<IEnumerable<SocksPair>>();

                pairs.Count().ShouldBe(2);
                pairs.ShouldAllBe(p => p.Colour == SocksColour.Black);
            }
        }
    }
}
