using System.Linq;
using System.Threading.Tasks;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Projects : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;

        private readonly TestConfig _config;

        public Projects(TestConfig config)
        {
            _client = new VstsRestClient(config.Organization, config.Token);
            _config = config;
        }

        /// <summary>
        /// Test if all projects have a Name
        /// </summary>
        [Fact]
        public void QueryProjects()
        {
            var definitions = _client.Get(Project.Projects());
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public async Task QueryProjectProperties()
        {
            var projects = _client.Get(Project.Projects());
            var firstProjectName = projects.First().Name;

            var id = await _client.GetAsync(Project.Properties(firstProjectName));
            id.ShouldNotBeNull();
        }

        [Fact]
        public async Task QuerySingleProjectWithNameShouldReturnAProject()
        {
            var project = await _client.GetAsync(Project.ProjectByName("TAS"));
            project.ShouldNotBeNull();
            project.Name.ShouldBe("TAS");
        }
    }
}