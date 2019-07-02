using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Projects : IClassFixture<TestConfig>
    {
        private readonly IVstsRestClient _client;

        private TestConfig _config;

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
            var definitions = _client.Get(Requests.Project.Projects());
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public async Task QueryProjectProperties()
        {
            var projects = _client.Get(Requests.Project.Projects());
            var firstProjectName = projects.First().Name;

            var id = await _client.GetAsync(Requests.Project.Properties(firstProjectName));
            id.ShouldNotBeNull();
        }

        [Fact]
        public async Task QuerySingleProjectWithNameShouldReturnAProject()
        {
            
            var project = await _client.GetAsync(Requests.Project.ProjectByName("TAS"));
            project.ShouldNotBeNull();
        }
    }
}