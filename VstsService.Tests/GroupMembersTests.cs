using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Tests;
using Shouldly;
using System;
using System.Linq;
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
    }
}