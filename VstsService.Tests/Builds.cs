using System.Linq;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class Builds : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private IVstsRestClient _client;

        public Builds(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }
        
        [Fact]
        [Trait("category", "integration")]
        public void QueryArtifacts()
        {
            var artifacts = _client.Get(Requests.Builds.Artifacts(_config.Project, _config.BuildId));
            artifacts.ShouldNotBeEmpty();

            var artifact = artifacts.First();
            artifact.Id.ShouldNotBe(0);
            
            artifact.Resource.ShouldNotBeNull();
            artifact.Resource.Type.ShouldBe("Container");
        }

        [Fact]
        [Trait("category", "integration")]
        public void QueryBuild()
        {
            var build = _client.Get(Requests.Builds.Build(_config.Project, _config.BuildId));
            build.ShouldNotBeNull();
            build.Id.ShouldNotBe(0);
            build.Definition.ShouldNotBeNull();
            build.Project.ShouldNotBeNull();
        }
    }
}