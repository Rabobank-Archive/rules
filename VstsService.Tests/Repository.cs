using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class Repository: IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _vsts;

        public Repository(TestConfig config)
        {
            this._config = config;
            _vsts = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task QueryRepository()
        {
            var definition = await _vsts.GetAsync(Requests.Repository.Repositories(_config.Project));
            var repositories = definition.ToList();
            repositories.ShouldNotBeEmpty();
            repositories.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            repositories.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            repositories.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Id));
            repositories.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Name));
            repositories.ShouldAllBe(e => !string.IsNullOrEmpty(e.DefaultBranch));
        }
    }
}
