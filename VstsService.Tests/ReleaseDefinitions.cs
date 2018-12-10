using Xunit;
using Shouldly;
using System.Net;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ReleaseDefinitions : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public ReleaseDefinitions(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryReleaseDefinitions()
        {
            var definitions = client.Get(Requests.Release.Definitions(config.Project));
            definitions.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void QueryReleaseDefinitionDetails()
        {
            var definition = client.Get(Requests.Release.Definition(config.Project, "2"));
            definition.Name.ShouldBe("demo SOx");
        }
    }
}
