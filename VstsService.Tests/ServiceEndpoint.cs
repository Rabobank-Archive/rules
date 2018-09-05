using Xunit;
using Shouldly;
using System.Net;

namespace SecurePipelineScan.VstsService.Tests
{
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
            var definition = Vsts.Execute(Requests.ServiceEndpoint.Endpoints(config.Project));

            definition.StatusCode.ShouldBe(HttpStatusCode.OK);
            definition.Data.Value.ShouldNotBeEmpty();
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            definition.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
        }

        [Fact]
        public void QueryServiceConnectionHistory()
        {
            var history = Vsts.Execute(Requests.ServiceEndpoint.History(config.Project, "975b3603-9939-4f22-a5a9-baebb39b5dad"));
            history.StatusCode.ShouldBe(HttpStatusCode.OK);
            history.Data.Value.ShouldNotBeEmpty();
            history.Data.Value.ShouldAllBe(e => e.Data != null);
            history.Data.Value.ShouldAllBe(e => e.Data.Definition != null);
            history.Data.Value.ShouldAllBe(e => !string.IsNullOrEmpty(e.Data.Definition.Id));
        }
    }
}
