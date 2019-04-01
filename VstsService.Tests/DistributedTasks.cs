using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class DistributedTasks : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public DistributedTasks(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void GetAllOrganizationalPools()
        {
            var orgPools = client.Get(Requests.DistributedTask.OrganizationalAgentPools());

            orgPools.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
            orgPools.ShouldAllBe(_ => !string.IsNullOrEmpty(_.PoolType));
        }

        [Fact]
        public void GetAgentPool()
        {
            var agentPool = client.Get(Requests.DistributedTask.AgentPool(config.AgentPoolId));
            agentPool.Name.ShouldBe(config.ExpectedAgentPoolName);
        }

        [Fact]
        public void GetAgentStatus()
        {
            var agentStatus = client.Get(Requests.DistributedTask.AgentPoolStatus(config.AgentPoolId));

            agentStatus.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }

        [Fact]
        public void GetTask()
        {
            var task = client.Get(Requests.DistributedTask.Tasks());

            task.ShouldAllBe(_ => !string.IsNullOrWhiteSpace(_.Id));
        }

        [Fact]
        public void QueryAgentQueueTest()
        {
            var response = client.Get(Requests.DistributedTask.AgentQueue(config.Project, 754));
            response.Id.ShouldBe(754);
            response.Pool.ShouldNotBeNull();
            response.Pool.Id.ShouldBe(9);
        }
    }
}