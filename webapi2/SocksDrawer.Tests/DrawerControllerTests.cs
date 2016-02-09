using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Results;
using ControllerTests;
using NSubstitute;
using Shouldly;
using SocksDrawer.Controllers;
using SocksDrawer.Models;
using Xunit;

namespace SocksDrawer.Tests
{
    public class DrawerControllerTests : ControllerTests.ApiControllerTestBase<ISocksDrawerRepository>
    {
        public DrawerControllerTests() : base(new ApiTestSetup<ISocksDrawerRepository>(Startup.GetContainer(), WebApiConfig.Register))
        {
          
        }


        // can POST new pair into drawer and get 201 in return
        [Fact]
        public void WhenPostNewPairToDrawer_ThenResponseIsCreatedAndSocksAreAddedToRepo()
        {
            var response = Post("api/drawer", new { colour = "black" });

            // assert response is 201
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            // assert id in location of response is not 0
            //var createdResponse = response.BodyAs<CreatedNegotiatedContentResult<SocksPair>>();
            //var idFromLocationUri = Regex.Match(createdResponse.Location.ToString(), @"/\d$").ToString();
            //Convert.ToInt32(idFromLocationUri).ShouldNotBe(0);
            //createdResponse.Content.Id.ShouldNotBe(0);
            // assert colour is 'black'
            response.BodyAs<SocksPair>().Colour.ShouldBe(SocksColour.Black);
            // assert repo method was called
            // todo: use real session
            //Session.Received().AddPair(Arg.Any<SocksPair>());
        }
    }
}
