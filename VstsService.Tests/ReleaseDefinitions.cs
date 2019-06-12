using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

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
        public async Task QueryReleaseDefinitions()
        {
            var definitions = await client.GetAsync(Requests.ReleaseManagement.Definitions(config.Project));
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public async Task QueryReleaseDefinitionDetails()
        {
            var definition = await client.GetAsync(Requests.ReleaseManagement.Definition(config.Project, config.ReleaseDefinitionId));
            definition.Name.ShouldBe(config.ReleaseDefinitionName);
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