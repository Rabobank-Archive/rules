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
        public void QueryProjectProperties()
        {
            var projects = _client.Get(Requests.Project.Projects());
            var firstProjectName = projects.First().Name;

            var id = _client.GetAsync(Requests.Project.Properties(firstProjectName));
            id.ShouldNotBeNull();
        }

        [Fact]
        public void QuerySingleProjectWithNameShouldReturnAProject()
        {
            var project = _client.GetAsync(Requests.Project.ProjectByName(_config.Project));
            project.ShouldNotBeNull();
        }
    }
}