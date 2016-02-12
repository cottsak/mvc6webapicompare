using System.Net;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Shouldly;
using WcoApi.Controllers;
using WcoApi.Domain.Permissions;
using Xunit;
using Xunit.Abstractions;

namespace WocApi.Tests
{
    public class UserControllerTests : LoggingTestBase
    {
        public UserControllerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public async Task WhenRegisterUser_ThenResponseIs200AndUserIsStored()
        {
            using (var host = new WcoDbSubcHost())
            {
                var response = await host.CreateClient().PostJsonAsync("/api/users/register", new NewUserCommand
                {
                    Name = "Dave",
                    Email = "dave@doofdoof.net.au",
                    Password = "DoofD00fD@ve",
                    Phone = null
                });

                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                (await host.GetContext().Set<User>().SingleOrDefaultAsync())
                    .Name.ShouldBe("Dave");
            }
        }
    }
}