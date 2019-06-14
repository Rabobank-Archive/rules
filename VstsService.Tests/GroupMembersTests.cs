using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.VstsService.Tests
{
    [Trait("category", "integration")]
    public class GroupMembersTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly IVstsRestClient _client;

        public GroupMembersTests(TestConfig config)
        {
            _config = config;
            _client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public async Task ReadGroupMembers()
        {
            var projectId = (await _client
                .GetAsync(Requests.Project.Projects()))
                .Single(x => x.Name == "SOx-compliant-demo").Id;

            var groupId = (await _client
                .GetAsync(Requests.Security.Groups(projectId)))
                .Identities
                .Single(x => x.FriendlyDisplayName == "Project Administrators")
                .TeamFoundationId;

            Guid.TryParse(groupId,  out _).ShouldBeTrue();

            var groupMembers = await _client
                .GetAsync(Requests.Security.GroupMembers(projectId, groupId));
            
            groupMembers.TotalIdentityCount.ShouldNotBe(0);
        }

        

        [Fact]
        public async Task AddAndRemoveGroupMember()
        {
            var projectId = (await _client
                .GetAsync(Requests.Project.Projects()))
                .Single(x => x.Name == "SOx-compliant-demo").Id;

            var groupId = (await _client
                .GetAsync(Requests.Security.Groups(projectId)))
                .Identities
                .Single(x => x.FriendlyDisplayName == "Project Administrators")
                .TeamFoundationId;
            
            await _client.PostAsync(
                Requests.Security.AddMember(_config.Project), 
                    new Requests.Security.AddMemberData(
                        new []{ "ab84d5a2-4b8d-68df-9ad3-cc9c8884270c" }, 
                        new [] { groupId }));

            await _client.PostAsync(
                Requests.Security.EditMembership(_config.Project),
                    new Requests.Security.RemoveMembersData(new[] { "ab84d5a2-4b8d-68df-9ad3-cc9c8884270c"}, groupId));
        }

        [Fact(Skip = "unable to delete created group with API")]
        public async Task CreateGroup()
        {
            await _client.PostAsync(
                Requests.Security.ManageGroup(_config.Project),
                    new Requests.Security.ManageGroupData
                    {
                        Name = "asdf"
                    });
        }
    }
}