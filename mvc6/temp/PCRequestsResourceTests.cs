using System.Threading.Tasks;
using Shouldly;
using WcoApi.Controllers;
using WcoApi.Domain;
using WocApi.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace WocApi.Tests
{
    public class PCRequestsResourceTests : LoggingTestBase
    {
        protected PCRequestsResourceTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Fact]
        public async Task GivenOnePCRInStore_WhenRequestById_ThenReturnsTheOnlyItemFromStore()
        {
            using (var fixture = new WocisDbSubcHost())
            {
                var session = fixture.GetSession();
                var pcr = new PolicyCancellationRequest("O085642", "WC09219110", "READIFY PTY LTD");
                session.Save(pcr);
                session.Flush();
           
                var response = await fixture.CreateClient().GetAsync($"api/pcrequests/{pcr.Id}");

                //response.StatusCode.ShouldBe(HttpStatusCode.OK);
                var view = response.BodyAs<PCRequestsController.PCRView>();
                view.PolicyNumber.ShouldBe(pcr.PolicyNumber);
                view.WorkCoverNumber.ShouldBe(pcr.WorkCoverNumber);
                view.EmployerName.ShouldBe(pcr.EmployerName);
            }
        }
    }
}

