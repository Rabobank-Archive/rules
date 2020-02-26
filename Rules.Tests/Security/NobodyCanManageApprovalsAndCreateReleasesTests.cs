using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using Shouldly;
using Xunit;
using response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanManageApprovalsAndCreateReleasesTests
    {
        [Theory]
        [InlineData(PermissionId.Allow, PermissionId.Allow, false)]
        [InlineData(PermissionId.Allow, PermissionId.Deny, true)]
        [InlineData(PermissionId.Deny, PermissionId.Allow, true)]
        [InlineData(PermissionId.Deny, PermissionId.Deny, true)]
        public async Task EachCombinationOfPermissionsProvidesCorrectReturnValue(
            int createReleasesPermissionId, int manageReleasesPermissionId, bool result)
        {
            var client = Substitute.For<IVstsRestClient>();
            var releasePipeline = new Fixture().Create<response.ReleaseDefinition>();

            InitializeLookupData(client, createReleasesPermissionId, manageReleasesPermissionId);

            var rule = new NobodyCanManageApprovalsAndCreateReleases(client);
            (await rule.EvaluateAsync("", releasePipeline)).ShouldBe(result);
        }

        private void InitializeLookupData(IVstsRestClient client, int createReleasesPermissionId,
            int manageApproversPermissionId)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            client.GetAsync(Arg.Any<IVstsRequest<response.ProjectProperties>>())
                .Returns(fixture.Create<response.ProjectProperties>());
            client.GetAsync(Arg.Any<IVstsRequest<response.ApplicationGroups>>())
                .Returns(fixture.Create<response.ApplicationGroups>());

            client.GetAsync(Arg.Any<IVstsRequest<response.PermissionsSetId>>()).Returns(new response.PermissionsSetId
            {
                Permissions = new[]
                {
                    new response.Permission
                    {
                        DisplayName = "Create releases",
                        PermissionBit = 64,
                        PermissionId = createReleasesPermissionId
                    },
                    new response.Permission
                    {
                        DisplayName = "Manage release approvers",
                        PermissionBit = 8,
                        PermissionId = manageApproversPermissionId
                    }
                }
            });
        }
    }
}