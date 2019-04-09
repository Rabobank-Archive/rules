
using System.Linq;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests
{
    public class ProjectAdministratorsOnlyContainsRaboAdministratorsTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private VstsRestClient _client;

        public ProjectAdministratorsOnlyContainsRaboAdministratorsTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }
        
        [Fact]
        public void Test()
        {
            var idProjectAdministrators = _client.Get(VstsService.Requests.ApplicationGroup.ApplicationGroups(_config.Project))
                .Identities.Single(p => p.FriendlyDisplayName == "Project Administrators").TeamFoundationId;
            
           var groupMembersProjectAdministrators = _client.Get(VstsService.Requests.ApplicationGroup.GroupMembers(_config.Project, idProjectAdministrators)).Identities;

           (groupMembersProjectAdministrators.Count(m => m.FriendlyDisplayName == "Rabobank Project Administrators") <= 1).ShouldBeTrue();
           (groupMembersProjectAdministrators.Count() <= 1).ShouldBeTrue();

        }
    }
}