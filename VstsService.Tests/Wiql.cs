using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using System.Linq;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.VstsService.Tests
{
    public class Wiql : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public Wiql(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task QueryByWiql_ShouldReturnResults()
        {
            // Arrange

            // Act
            var result = await _client.PostAsync(WorkItemTracking.QueryByWiql(_config.Project, 1),
                new QueryByWiql("select [System.Id] from WorkItems"));

            // Assert
            result.ShouldNotBeNull();
            result.WorkItems.ShouldNotBeEmpty();
            result.WorkItems.First().Id.ShouldNotBe(default);
        }
    }
}