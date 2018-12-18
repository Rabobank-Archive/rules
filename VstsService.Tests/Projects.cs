using System.Linq;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Projects : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public Projects(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        /// <summary>
        /// Test if all projects have a Name
        /// </summary>
        [Fact]
        public void QueryProjects()
        {
            var definitions = client.Get(Requests.Project.Projects());
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void QueryProjectProperties()
        {
            var projects = client.Get(Requests.Project.Projects());
            var firstProjectName = projects.First().Name;

            var id = client.Get(Requests.Project.ProjectProperties(firstProjectName));
            id.ShouldNotBeNull();
        }
    }
}