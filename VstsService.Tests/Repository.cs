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
            var definition = (await _vsts.GetAsync(Requests.Repository.Repositories(_config.Project))).ToList();
            definition.ShouldNotBeEmpty();
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Name));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Id));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Id));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.Project.Name));
            definition.ShouldAllBe(e => !string.IsNullOrEmpty(e.DefaultBranch));
        }
    }
}
