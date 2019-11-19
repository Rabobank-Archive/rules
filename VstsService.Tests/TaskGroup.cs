using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    public class TaskGroup : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public TaskGroup(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(_config.Organization, _config.Token);
        }

        [Fact]
        [Trait("category", "integration")]
        public async Task GetTaskGroupById()
        {
            var response = await _client.GetAsync(Requests.TaskGroup.TaskGroupById(_config.Project, "df6aa8e5-82dc-468c-a794-a7990523363d"));
            response.ShouldNotBeNull();
            response.Value.FirstOrDefault().Tasks.ShouldNotBeNull();
            response.Value.FirstOrDefault().Tasks.FirstOrDefault().Task.Id.ShouldNotBeNull();
        }
    }
}