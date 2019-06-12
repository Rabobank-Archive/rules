using Shouldly;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task QueryArtifacts()
        {
            var artifacts = (await _client.GetAsync(Requests.Builds.Artifacts(_config.Project, _config.BuildId))).ToList();
            artifacts.ShouldNotBeEmpty();

            var artifact = artifacts.First();
            artifact.Id.ShouldNotBe(0);

            artifact.Resource.ShouldNotBeNull();
            artifact.Resource.Type.ShouldBe("Container");
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task QueryBuild()
        {
            var build = await _client.GetAsync(Requests.Builds.Build(_config.Project, _config.BuildId));
            build.ShouldNotBeNull();
            build.Id.ShouldNotBe(0);
            build.Definition.ShouldNotBeNull();
            build.Project.ShouldNotBeNull();
        }

        [Fact]
        public async Task QueryBuildDefinitionsReturnsBuildDefinitions()
        {
            var projectId = (await _client.GetAsync(Requests.Project.Properties(_config.Project))).Id;

            var buildDefinitions = (await _client.GetAsync(Requests.Builds.BuildDefinitions(projectId))).ToList();

            buildDefinitions.ShouldNotBeNull();
            buildDefinitions.First().Id.ShouldNotBeNull();
        }

        [Fact]
        public async Task QueryBuildDefinitionsReturnsBuildDefinitionsWithExtendedProperties()
        {
            var projectId = (await _client.GetAsync(Requests.Project.Properties(_config.Project))).Id;

            var buildDefinitions = await _client.GetAsync(Requests.Builds.BuildDefinitions(projectId, true).AsJson());

            buildDefinitions.ShouldNotBeNull();
            buildDefinitions.SelectTokens("value[*].process").Count().ShouldBeGreaterThan(0);
        }
    }
}