using Xunit;
using Shouldly;
using System.Net;
using System.Linq;
using RestSharp;

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
            var endpoints = Vsts.Get(Requests.ServiceEndpoint.Endpoints(config.Project));
            endpoints.ShouldNotBeEmpty();

            var endpoint = endpoints.First();
            endpoint.Name.ShouldNotBeNullOrEmpty();
            endpoint.Id.ShouldNotBeNullOrEmpty();
            endpoint.Type.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void QueryServiceConnectionHistory()
        {
            //your user needs to be endpoint admin in the project you are running this.
            //your PAT needs "ALL scope". selecting all checkboxes does not work.

            var history = Vsts.Get(Requests.ServiceEndpoint.History(config.Project, "975b3603-9939-4f22-a5a9-baebb39b5dad"));
            history.ShouldNotBeEmpty();
            history.ShouldAllBe(e => e.Data != null);
            history.ShouldAllBe(e => e.Data.Definition != null);
            history.ShouldAllBe(e => !string.IsNullOrEmpty(e.Data.Definition.Id));

            var data = history.First().Data;
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
