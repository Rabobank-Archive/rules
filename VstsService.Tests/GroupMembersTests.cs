using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Requests = SecurePipelineScan.VstsService.Requests;

namespace VstsService.Tests
{
    [Trait("category", "integration")]
    public class GroupMembersTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig config;
        private readonly IVstsRestClient client;

        public GroupMembersTests(TestConfig config)
        {
            this.config = config;
            client = new VstsRestClient(config.Organization, config.Token);
        }

        [Fact]
        public void ReadGroupMembers()
        {
            var projectId = client
                .Get(Requests.Project.Projects())
                .Single(x => x.Name == "SOx-compliant-demo").Id;

            var groupId = client
                .Get(Requests.Security.Groups(projectId))
                .Identities
                .Single(x => x.FriendlyDisplayName == "Project Administrators")
                .TeamFoundationId;

            Guid.TryParse(groupId,  out _).ShouldBeTrue();

            var groupMembers = client
                .Get(Requests.Security.GroupMembers(projectId, groupId));
            
            groupMembers.TotalIdentityCount.ShouldNotBe(0);
        }

        

        [Fact]
        public void AddAndRemoveGroupMember()
        {
            var projectId = client
                .Get(Requests.Project.Projects())
                .Single(x => x.Name == "SOx-compliant-demo").Id;

            var groupId = client
                .Get(Requests.Security.Groups(projectId))
                .Identities
                .Single(x => x.FriendlyDisplayName == "Project Administrators")
                .TeamFoundationId;
            
            client.Post(
                Requests.Security.AddMember(config.Project), 
                    new Requests.Security.AddMemberData(
                        new []{ "ab84d5a2-4b8d-68df-9ad3-cc9c8884270c" }, 
                        new [] { groupId }));

            client.Post(
                Requests.Security.EditMembership(config.Project),
                    new Requests.Security.RemoveMembersData(new[] { "ab84d5a2-4b8d-68df-9ad3-cc9c8884270c"}, groupId));
        }

        [Fact(Skip = "unable to delete created group with API")]
        public void CreateGroup()
        {
            client.Post(
                Requests.Security.ManageGroup(config.Project),
                    new Requests.Security.ManageGroupData
                    {
                        Name = "asdf"
                    });
        }
    }
}