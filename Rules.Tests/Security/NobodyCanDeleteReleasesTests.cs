using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using SecurePipelineScan.Rules.PermissionBits;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using Response = SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteReleasesTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers =  true });

        [Theory, CombinatorialData]
        public async Task EvaluateFalse(
            [CombinatorialValues(PermissionId.Allow, PermissionId.AllowInherited)] int permissionId, 
            [CombinatorialValues(
                Release.DeleteReleases, 
                Release.AdministerReleasePermissions, 
                Release.DeleteReleasePipelines)] int permissionBit)
        {
            // Arrange 
            CustomizePermission(permissionId, permissionBit);

            // Act
            var rule = new NobodyCanDeleteReleases(_fixture.Create<IVstsRestClient>());
            var result = await rule.EvaluateAsync(_fixture.Create<string>(), _fixture.Create<Response.ReleaseDefinition>());
            
            // Assert
            result
                .Value
                .ShouldBeFalse();
        }
        
        [Theory, CombinatorialData]
        public async Task EvaluateTrue(
            [CombinatorialValues(PermissionId.Deny, PermissionId.DenyInherited, PermissionId.NotSet)] int permissionId, 
            [CombinatorialValues(
                Release.DeleteReleases, 
                Release.AdministerReleasePermissions, 
                Release.DeleteReleasePipelines)] int permissionBit)
        {
            // Arrange 
            CustomizePermission(permissionId, permissionBit);

            // Act
            var rule = new NobodyCanDeleteReleases(_fixture.Create<IVstsRestClient>());
            var result = await rule.EvaluateAsync(_fixture.Create<string>(), _fixture.Create<Response.ReleaseDefinition>());
            
            // Assert
            result
                .Value
                .ShouldBeTrue();
        }
        
        [Theory]
        [InlineData("Project Collection Administrators")]
        public async Task Exclude(string group)
        {
            // Arrange
            CustomizePermission(PermissionId.Allow, Release.AdministerReleasePermissions);
            CustomizeApplicationGroup(group);

            // Act
            var rule = new NobodyCanDeleteReleases(_fixture.Create<IVstsRestClient>());
            var result = await rule.EvaluateAsync(_fixture.Create<string>(), _fixture.Create<Response.ReleaseDefinition>());
            
            // Assert
            result
                .Value
                .ShouldBeTrue();
        }
        
        [Fact]
        public async Task Reconcile()
        {
            // Arrange
            CustomizePermission(PermissionId.Allow, Release.DeleteReleases);
            var client = _fixture.Create<IVstsRestClient>();

            // Act
            var rule = new NobodyCanDeleteReleases(client);
            await rule.ReconcileAsync(_fixture.Create<string>(), _fixture.Create<string>());
            
            // Assert
            await client
                .Received(_fixture.RepeatCount * _fixture.RepeatCount) // identities * permissions
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),Arg.Any<Permissions.UpdateWrapper>());
        }
        
        private void CustomizeApplicationGroup(string group) =>
            _fixture.Customize<Response.ApplicationGroup>(ctx => ctx
                .With(x => x.FriendlyDisplayName, @group));

        private void CustomizePermission(int permissionId, int permissionBit) =>
            _fixture.Customize<Response.Permission>(ctx => ctx
                .With(x => x.PermissionId, permissionId)
                .With(x => x.PermissionBit, permissionBit));
    }
}