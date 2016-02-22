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
                var response = await host.CreateClient().PostJsonAsync("api/drawer", new { colour = "black" });

                // assert response is 201
                response.StatusCode.ShouldBe(HttpStatusCode.Created);
                response.Headers.Location.ToString().ShouldContain("api/drawer");
                // assert pair is in store by itself
                var pairFromStore = host.Session.Query<SocksPair>().Single();
                pairFromStore.Id.ShouldNotBe(0);
                pairFromStore.Colour.ShouldBe(SocksColour.Black);
            }
        }

        [Fact]
        public async Task GivenTwoBlackPairsInStore_WhenGetAllPairs_ThenTwoAreReturned()
        {
            using (var host = new SubCTestHost())
            {
                var twoPairs = new[] { new SocksPair(SocksColour.Black), new SocksPair(SocksColour.Black), };
                twoPairs.ToList().ForEach(p => host.Session.Save(p));
                host.Session.Flush();

                var pairs = (await host.CreateClient().GetAsync("api/drawer/socks")).BodyAs<IEnumerable<SocksPair>>();

                pairs.Count().ShouldBe(2);
                pairs.ShouldAllBe(p => p.Colour == SocksColour.Black);
            }
        }

        [Fact]
        public void GivenTwoBlackPairsInStore_WhenDeleteOne_ThenOnlyOneRemains()
        {
            using (var host = new SubCTestHost())
            {
                host.Session.Save(new SocksPair(SocksColour.Black));
                var pairTwo = new SocksPair(SocksColour.Black);
                host.Session.Save(pairTwo);
                host.Session.Flush();

                var response =  host.CreateClient().DeleteAsync($"api/drawer/{pairTwo.Id}").Result;

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                host.Session.Query<SocksPair>().Count().ShouldBe(1);
                host.Session.Query<SocksPair>().SingleOrDefault(p => p.Id == pairTwo.Id).ShouldBeNull();
            }
        }
    }

    public class AnotherTestCollection      // only doing this for parallel xunit tests https://xunit.github.io/docs/running-tests-in-parallel.html
    {
        [Fact]
        public async Task GivenOnePairInStore_WhenGetById_ThenReturnOnlyTheOne()
        {
            using (var host = new SubCTestHost())
            {
                var pair = new SocksPair(SocksColour.White);
                host.Session.Save(pair);
                host.Session.Flush();

                var response = (await host.CreateClient().GetAsync($"api/drawer/{pair.Id}")).BodyAs<SocksPair>();

                response.Colour.ShouldBe(pair.Colour);
                response.Id.ShouldBe(pair.Id);
            }
        }

        [Fact]
        public async Task GivenOneBlackAndSixWhiteSocks_WhenPostWhite_ThenFailWithForbidden()
        {
            using (var host = new SubCTestHost())
            {
                var allPairsInStore = new[]
                    {
                        new SocksPair(SocksColour.Black),
                        new SocksPair(SocksColour.White),
                        new SocksPair(SocksColour.White),
                        new SocksPair(SocksColour.White),
                        new SocksPair(SocksColour.White),
                        new SocksPair(SocksColour.White),
                        new SocksPair(SocksColour.White),
                    };
                allPairsInStore.ToList().ForEach(p => host.Session.Save(p));
                host.Session.Flush();

                var response = await host.CreateClient().PostJsonAsync("api/drawer", new { colour = "white" });

                response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
                response.BodyAs<string>().ShouldContain("6");
            }
        }
    }
}
