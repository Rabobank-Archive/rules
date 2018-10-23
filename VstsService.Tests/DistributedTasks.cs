using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System.Net;
using Xunit;

namespace VstsService.Tests
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
            var orgPools = client.Execute(Requests.DistributedTask.OrganizationalAgentPools());

            orgPools.StatusCode.ShouldBe(HttpStatusCode.OK);
            orgPools.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
            orgPools.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.PoolType));
        }

        [Fact]
        public void GetAgentPool()
        {
            var agentPool = client.Execute(Requests.DistributedTask.AgentPool(119));

            agentPool.StatusCode.ShouldBe(HttpStatusCode.OK);
            agentPool.Data.Name.ShouldBe("Rabo-Build-Azure-Windows");
        }

        [Fact]
        public void GetAgentStatus()
        {
            var agentStatus = client.Execute(Requests.DistributedTask.AgentPoolStatus(119));

            agentStatus.StatusCode.ShouldBe(HttpStatusCode.OK);
            agentStatus.Data.Value.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }
    }
}