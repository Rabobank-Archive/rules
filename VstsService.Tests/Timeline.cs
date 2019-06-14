using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class Timeline : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private IVstsRestClient _client;

        public Timeline(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public async void QueryTimeline()
        {
            var timeline = await _client.GetAsync(Requests.Builds.Timeline(_config.Project, _config.BuildId));
            timeline.ShouldNotBeNull();
        }
    }
}