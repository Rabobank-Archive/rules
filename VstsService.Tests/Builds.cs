using Shouldly;
using System.Linq;
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
            build.Result.ShouldNotBeNull();
        }

        [Fact]
        public void QueryBuildDefinitionsReturnsBuildDefinitions()
        {
            var projectId = _client.Get(Requests.Project.Properties(_config.Project)).Id;

            var buildDefinitions = _client.Get(Requests.Builds.BuildDefinitions(projectId));

            buildDefinitions.ShouldNotBeNull();
            buildDefinitions.First().Id.ShouldNotBeNull();
        }

        [Fact]
        public void QueryBuildDefinitionsReturnsBuildDefinitionsWithExtendedProperties()
        {
            var projectId = _client.Get(Requests.Project.Properties(_config.Project)).Id;

            var buildDefinitions = _client.Get(Requests.Builds.BuildDefinitions(projectId, true).AsJson());

            buildDefinitions.ShouldNotBeNull();
            buildDefinitions.SelectTokens("value[*].process").Count().ShouldBeGreaterThan(0);
        }
    }
}