using System.Linq;
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
        public void QueryReleaseDefinitions()
        {
            var definitions = client.Get(Requests.ReleaseManagement.Definitions(config.Project));
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void QueryReleaseDefinitionDetails()
        {
            var definition = client.Get(Requests.ReleaseManagement.Definition(config.Project, config.ReleaseDefinitionId));
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