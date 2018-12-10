using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System.Net;
using RestSharp;
using Xunit;
using Requests = SecurePipelineScan.VstsService.Requests;

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
            var orgPools = client.Get(Requests.DistributedTask.OrganizationalAgentPools());

            orgPools.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
            orgPools.ShouldAllBe(_ => !string.IsNullOrEmpty(_.PoolType));
        }

        [Fact]
        public void GetAgentPool()
        {
            var agentPool = client.Get(Requests.DistributedTask.AgentPool(119));

            agentPool.Name.ShouldBe("Rabo-Build-Azure-Windows");
        }

        [Fact]
        public void GetAgentStatus()
        {
            var agentStatus = client.Get(Requests.DistributedTask.AgentPoolStatus(119));

            agentStatus.ShouldAllBe(_ => !string.IsNullOrEmpty(_.Name));
        }
    }
}