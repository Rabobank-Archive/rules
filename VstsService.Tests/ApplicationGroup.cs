using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class ApplicationGroup : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public ApplicationGroup(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task QueryApplicationGroups()
        {
            var identity = await _client.GetAsync(Requests.ApplicationGroup.ApplicationGroups(_config.Project));
            identity.ShouldNotBeNull();
            identity.Identities.ShouldNotBeEmpty();

            var group = identity.Identities.First();
            group.DisplayName.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public async Task ExplicitIdentitiesShouldGetIdentities()
        {
            string projectId = "53410703-e2e5-4238-9025-233bd7c811b3";
            string nameSpaceId = "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87";
            string repositoryId = "3167b64e-c72b-4c55-84eb-986ac62d0dec";

            var explicitIdentities = await _client.GetAsync(Requests.ApplicationGroup.ExplicitIdentitiesRepos(projectId, nameSpaceId, repositoryId));
            explicitIdentities.ShouldNotBeNull();
        }

        [Fact]
        public async Task ExplicitIdentitiesForBuildDefinitionShouldGetIdentities()
        {
            string projectId = "53410703-e2e5-4238-9025-233bd7c811b3";
            string nameSpaceId = "33344d9c-fc72-4d6f-aba5-fa317101a7e9";
            string buildPipelineId = "2";

            var explicitIdentities = await _client.GetAsync(Requests.ApplicationGroup.ExplicitIdentitiesPipelines(projectId, nameSpaceId, buildPipelineId));
            explicitIdentities.ShouldNotBeNull();
            explicitIdentities.Identities.ShouldNotBeEmpty();
        }
    }
}