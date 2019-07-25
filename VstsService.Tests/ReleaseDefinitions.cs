using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ReleaseDefinitions : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public ReleaseDefinitions(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void QueryReleaseDefinitions()
        {
            var definitions = _client.Get(Requests.ReleaseManagement.Definitions(_config.Project));
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public async Task QueryReleaseDefinitionDetails()
        {
            var definition = await _client.GetAsync(Requests.ReleaseManagement.Definition(_config.Project, _config.ReleaseDefinitionId));
            definition.Name.ShouldBe(_config.ReleaseDefinitionName);
            definition.Links.ShouldNotBeNull();
            definition.Environments.ShouldNotBeEmpty();

            var environment = definition.Environments.First();
            environment.Name.ShouldNotBeEmpty();
            environment.DeployPhases.ShouldNotBeEmpty();

            var phase = environment.DeployPhases.First();
            phase.WorkflowTasks.ShouldNotBeEmpty();

            var task = phase.WorkflowTasks.First();
            task.Name.ShouldNotBeEmpty();
        }
    }
}