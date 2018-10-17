using Xunit;
using Shouldly;
using System.Net;
using System.Linq;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ServiceEndpoint : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient Vsts;

        public ServiceEndpoint(TestConfig config)
        {
            this.config = config;
            Vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryServiceConnections()
        {
            var endpoints = Vsts.Execute(Requests.ServiceEndpoint.Endpoints(config.Project));

            endpoints.StatusCode.ShouldBe(HttpStatusCode.OK);
            endpoints.Data.Value.ShouldNotBeEmpty();

            var endpoint = endpoints.Data.Value.First();
            endpoint.Name.ShouldNotBeNullOrEmpty();
            endpoint.Id.ShouldNotBeNullOrEmpty();
            endpoint.Type.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void QueryServiceConnectionHistory()
        {
            //your user needs to be endpoint admin in the project you are running this.
            //your PAT needs "ALL scope". selecting all checkboxes does not work.

            var history = Vsts.Execute(Requests.ServiceEndpoint.History(config.Project, "975b3603-9939-4f22-a5a9-baebb39b5dad"));
            history.StatusCode.ShouldBe(HttpStatusCode.OK);
            history.Data.Value.ShouldNotBeEmpty();
            history.Data.Value.ShouldAllBe(e => e.Data != null);
            history.Data.Value.ShouldAllBe(e => e.Data.Definition != null);
            history.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Data.Definition.Id));

            var data = history.Data.Value.First().Data;
            data.Id.ShouldNotBe(0);
            data.Owner.ShouldNotBeNull();
            data.Owner.Id.ShouldNotBe(0);
            data.Owner.Name.ShouldNotBeNullOrEmpty();
            data.Owner.Links.ShouldNotBeNull();
            data.Owner.Links.Web.ShouldNotBeNull();
            data.Owner.Links.Self.ShouldNotBeNull();
            data.Owner.Links.Self.Href.ShouldNotBeNull();
            data.PlanType.ShouldNotBeNullOrEmpty();
        }
    }
}
