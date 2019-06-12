using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Projects : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public Projects(TestConfig config)
        {
            this._config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        /// <summary>
        /// Test if all projects have a Name
        /// </summary>
        [Fact]
        public async Task QueryProjects()
        {
            var definitions = await _client.GetAsync(Requests.Project.Projects());
            definitions.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public async Task QueryProjectProperties()
        {
            var projects = await _client.GetAsync(Requests.Project.Projects());
            var firstProjectName = projects.First().Name;

            var id = _client.GetAsync(Requests.Project.Properties(firstProjectName));
            id.ShouldNotBeNull();
        }
    }
}