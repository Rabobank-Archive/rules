using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using SecurePipelineScan.VstsService.Requests;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanManageApprovalsAndCreateReleasesTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanManageApprovalsAndCreateReleasesTests(TestConfig config)
        {
            _config = config;
        }

        [Theory]
        [InlineData(PermissionId.Allow, PermissionId.Allow, false)]
        [InlineData(PermissionId.Allow, PermissionId.Deny, true)]
        [InlineData(PermissionId.Deny, PermissionId.Allow, true)]
        [InlineData(PermissionId.Deny, PermissionId.Deny, true)]
        public async Task EachCombinationOfPermissionsProvidesCorrectReturnValue(
            int createReleasesPermissionId, int manageReleasesPermissionId, bool result)
        {
            var client = Substitute.For<IVstsRestClient>();
            var releasePipeline = Substitute.For<response.ReleaseDefinition>();

            InitializeLookupData(client, createReleasesPermissionId, manageReleasesPermissionId);

            var rule = new NobodyCanManageApprovalsAndCreateReleases(client);
            (await rule.EvaluateAsync(_config.Project, releasePipeline)).ShouldBe(result);
        }

        [Fact]
        public async Task EvaluateReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;
            var releasePipeline = await client.GetAsync(ReleaseManagement.Definition(_config.Project, "1"))
                .ConfigureAwait(false);

            var rule = new NobodyCanManageApprovalsAndCreateReleases(client);
            (await rule.EvaluateAsync(projectId, releasePipeline)).ShouldBeTrue();
        }

        [Fact]
        public async Task ReconcileReleaseIntegrationTest()
        {
            var client = new VstsRestClient(_config.Organization, _config.Token);
            var projectId = (await client.GetAsync(Project.Properties(_config.Project))).Id;

            var rule = new NobodyCanManageApprovalsAndCreateReleases(client) as IReconcile;
            await rule.ReconcileAsync(projectId, "1");
        }

        private void InitializeLookupData(IVstsRestClient client, int createReleasesPermissionId, int manageApproversPermissionId)
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            client.GetAsync(Arg.Any<IVstsRequest<response.ProjectProperties>>()).Returns(fixture.Create<response.ProjectProperties>());
            client.GetAsync(Arg.Any<IVstsRequest<response.ApplicationGroups>>()).Returns(fixture.Create<response.ApplicationGroups>());

            client.GetAsync(Arg.Any<IVstsRequest<response.PermissionsSetId>>()).Returns(new response.PermissionsSetId()
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
                    },
                }
            });
        }
    }
}