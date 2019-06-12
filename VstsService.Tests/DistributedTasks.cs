using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class DistributedTasks : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public DistributedTasks(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task GetAllOrganizationalPools()
        {
            var orgPools = (await _client.GetAsync(Requests.DistributedTask.OrganizationalAgentPools())).ToList();

            orgPools.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
            orgPools.ShouldAllBe(_ => !string.IsNullOrEmpty(_.PoolType));
        }

        [Fact]
        public async Task GetAgentPool()
        {
            var agentPool = await _client.GetAsync(Requests.DistributedTask.AgentPool(_config.AgentPoolId));
            agentPool.Name.ShouldBe(_config.ExpectedAgentPoolName);
        }

        [Fact]
        public async Task GetAgentStatus()
        {
            var agentStatus = await _client.GetAsync(Requests.DistributedTask.AgentPoolStatus(_config.AgentPoolId));

            agentStatus.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public async Task GetTask()
        {
            var task = await _client.GetAsync(Requests.DistributedTask.Tasks());

            task.ShouldAllBe(_ => !string.IsNullOrWhiteSpace(_.Id));
        }

        [Fact]
        public async Task QueryAgentQueueTest()
        {
            var response = await _client.GetAsync(Requests.DistributedTask.AgentQueue(_config.Project, 754));
            response.Id.ShouldBe(754);
            response.Pool.ShouldNotBeNull();
            response.Pool.Id.ShouldBe(9);
        }
    }
}