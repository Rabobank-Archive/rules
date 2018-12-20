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
            string projectName = "SOx-compliant-demo";
            string groupName = "Project Administrators";

            var projectId = client.Get(Requests.Project.Projects()).Value.
                Single(x => x.Name == projectName).Id;

            var groupId = client.Get(Requests.Security.Groups(projectId)).Identities.
                Single(x => x.FriendlyDisplayName == groupName).TeamFoundationId;

            Guid.TryParse(groupId,  out var guidResult).ShouldBeTrue();

            var groupMembers = client.Get(Requests.Security.GroupMembers(projectId, groupId));
            groupMembers.TotalIdentityCount.ShouldBe(1);
        }
    }
}