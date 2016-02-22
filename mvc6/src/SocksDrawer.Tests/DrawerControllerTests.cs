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
    public class DrawerControllerTests : LocalDbSubCTest
    {
        [Fact]
        public async Task WhenPostNewPairToDrawer_ThenResponseIsCreatedAndPairIsOnlyOneInStore()
        {
            var response = await Client.PostJsonAsync("api/drawer", new { colour = "black" });

            // assert response is 201
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Headers.Location.ToString().ShouldContain("api/drawer");
            // assert pair is in store by itself
            var pairFromStore = Session.Query<SocksPair>().Single();
            pairFromStore.Id.ShouldNotBe(0);
            pairFromStore.Colour.ShouldBe(SocksColour.Black);
        }

        [Fact]
        public async Task GivenTwoBlackPairsInStore_WhenGetAllPairs_ThenTwoAreReturned()
        {
            var twoPairs = new[] { new SocksPair(SocksColour.Black), new SocksPair(SocksColour.Black), };
            twoPairs.ToList().ForEach(p => Session.Save(p));
            Session.Flush();

            var pairs = (await Client.GetAsync("api/drawer/socks")).BodyAs<IEnumerable<SocksPair>>();

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

            var response = Client.DeleteAsync($"api/drawer/{pairTwo.Id}").Result;

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            Session.Query<SocksPair>().Count().ShouldBe(1);
            Session.Query<SocksPair>().SingleOrDefault(p => p.Id == pairTwo.Id).ShouldBeNull();
        }
    }


    public class AnotherTestCollection : LocalDbSubCTest    // only doing this for parallel xunit tests https://xunit.github.io/docs/running-tests-in-parallel.html
    {
        [Fact]
        public async Task GivenOnePairInStore_WhenGetById_ThenReturnOnlyTheOne()
        {
            var pair = new SocksPair(SocksColour.White);
            Session.Save(pair);
            Session.Flush();

            var response = (await Client.GetAsync($"api/drawer/{pair.Id}")).BodyAs<SocksPair>();

            response.Colour.ShouldBe(pair.Colour);
            response.Id.ShouldBe(pair.Id);
        }

        [Fact]
        public async Task GivenOneBlackAndSixWhiteSocks_WhenPostWhite_ThenFailWithForbidden()
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
            allPairsInStore.ToList().ForEach(p => Session.Save(p));
            Session.Flush();

            var response = await Client.PostJsonAsync("api/drawer", new { colour = "white" });

            response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
            response.BodyAs<string>().ShouldContain("6");
        }
    }
}
