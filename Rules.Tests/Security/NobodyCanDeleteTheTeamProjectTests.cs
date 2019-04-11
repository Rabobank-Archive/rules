using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests
{
    public class NobodyCanDeleteTheTeamProjectTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        public NobodyCanDeleteTheTeamProjectTests(TestConfig config)
        {
            _config = config;
        }
        
        [Fact]
        public void IntegrationTest()
        {
            var rule = new NobodyCanDeleteTheTeamProject(new VstsRestClient(_config.Organization, _config.Token));
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }
        
        [Fact]
        public void GivenProjectAdministratorsMembersEmpty_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);           
            InitializeProjectAdministratorsLookup(client);            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }

        [Fact]
        public void GivenProjectAdministratorsHasOnlyRabobankProjectAdministrators_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);           
            InitializeProjectAdministratorsLookup(client);            
            InitializeMembersLookup(client, new Response.ApplicationGroup{FriendlyDisplayName = "Rabobank Project Administrators"});
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }

        [Fact]
        public void GivenProjectAdministratorsHasOtherMember_WhenEvaluate_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeProjectAdministratorsLookup(client);
            InitializeMembersLookup(client, new Response.ApplicationGroup());
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeFalse();
        }

        [Fact]
        public void GivenOnlyProjectAdministratorHasPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, new Response.Permission{ DisplayName = "Delete team project", PermissionId = PermissionId.Allow});
            InitializeProjectAdministratorsLookup(client);            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }

        [Fact]
        public void GivenOtherApplicationGroupHasPermissionToDeleteTeamProject_WhenEvaluate_ThenFalse()
        {
            
        }

        [Fact]
        public void GivenProjectAdministratorHasNoPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
        }

        private static void InitializeProjectAdministratorsLookup(IVstsRestClient client)
        {
            client
                .Get(Arg.Is<IVstsRestRequest<Response.ApplicationGroups>>(x =>
                    x.Uri.Contains("ReadScopedApplicationGroupsJson")))
                .Returns(new Response.ApplicationGroups
                {
                    Identities = new[] {new Response.ApplicationGroup {FriendlyDisplayName = "Project Administrators"}}
                });
        }

        private static void InitializeMembersLookup(IVstsRestClient client, params Response.ApplicationGroup[] members)
        {
            client
                .Get(Arg.Is<IVstsRestRequest<Response.ApplicationGroups>>(x =>
                    x.Uri.Contains("ReadGroupMembers")))
                .Returns(new Response.ApplicationGroups
                {
                    Identities = members
                });
        }
        
        private static void InitializePermissions(IVstsRestClient client, params Response.Permission[] permissions)
        {
            client.Get(Arg.Any<IVstsRestRequest<Response.PermissionsProjectId>>())
                .Returns(new Response.PermissionsProjectId
                    {Security = new Response.PermissionsSetId {Permissions = permissions}});
        }
    }
}