using System.Collections.Generic;
using System.Linq;
using Moq;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using Shouldly;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheTeamProjectTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private Response.ApplicationGroup _pa = new Response.ApplicationGroup {FriendlyDisplayName = "Project Administrators", TeamFoundationId = "1234"};
        private readonly Response.ApplicationGroup _rpa = new Response.ApplicationGroup{FriendlyDisplayName = "Rabobank Project Administrators", TeamFoundationId = "adgasge"};

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
            InitializeApplicationGroupsLookup(client, new Response.ApplicationGroup {FriendlyDisplayName = "Project Administrators"});            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }

        [Fact]
        public void GivenProjectAdministratorsHasOnlyRabobankProjectAdministrators_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);           
            InitializeApplicationGroupsLookup(client, _pa);            
            InitializeMembersLookup(client, _rpa);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }

        [Fact]
        public void GivenProjectAdministratorsHasOtherMember_WhenEvaluate_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client, new Response.ApplicationGroup());
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeFalse();
        }

        [Fact]
        public void GivenOnlyProjectAdministratorHasPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, new Response.Permission{ DisplayName = "Delete team project", PermissionId = PermissionId.Allow});
            InitializeApplicationGroupsLookup(client, _pa);            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }

        [Fact]
        public void GivenContributorsHasPermissionToDeleteTeamProject_WhenEvaluate_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, new Response.Permission{ DisplayName = "Delete team project", PermissionId = PermissionId.Allow});
            InitializeApplicationGroupsLookup(client, new Response.ApplicationGroup {FriendlyDisplayName = "Contributors"});            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeFalse();
        }

        [Fact]
        public void GivenProjectAdministratorHasNoPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, new Response.Permission());
            InitializeApplicationGroupsLookup(client, _pa);            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Evaluate(_config.Project).ShouldBeTrue();
        }

        [Fact]
        public void GivenProjectAdministratorsGroupContainsOtherMembers_WhenFix_ThenMembersAreRemoved()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client, 
                new Response.ApplicationGroup { TeamFoundationId = "asdf"},
                new Response.ApplicationGroup { TeamFoundationId = "gsdgs"});

            var data = new List<object>();
            client
                .Post(Arg.Do<IVstsPostRequest<object>>(x => data.Add(x.Body)));
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Fix(_config.Project);

            data
                .OfType<VstsService.Requests.Security.RemoveMembersData>()
                .ShouldContain(x => x.RemoveItemsJson.Contains("asdf") &&
                                    x.RemoveItemsJson.Contains("gsdgs") &&
                                    x.GroupId == "1234");
        }
        
        [Fact]
        public void GivenProjectAdministratorsGroupContainsOtherMembers_WhenFix_ThenMembersAreAddedToRabobankProjectAdministratorsGroup()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client, 
                _rpa,
                new Response.ApplicationGroup { TeamFoundationId = "asdf"},
                new Response.ApplicationGroup { TeamFoundationId = "gsdgs"});
            
            var data = new List<object>();
            client
                .Post(Arg.Do<IVstsPostRequest<object>>(x => data.Add(x.Body)));
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Fix(_config.Project);

            data
                .OfType<VstsService.Requests.Security.AddMemberData>()
                .ShouldContain(x => x.GroupsToJoinJson.Contains(_rpa.TeamFoundationId) &&
                                    x.ExistingUsersJson.Contains("asdf") && 
                                    x.ExistingUsersJson.Contains("gsdgs"));
        }
        
        [Fact]
        public void GivenProjectAdministratorsGroupContainsRabobankAdministratorsGroups_WhenFix_ThenThatMemberIsNotRemoved()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client, _rpa);
            
            var data = new List<object>();
            client
                .Post(Arg.Do<IVstsPostRequest<object>>(x => data.Add(x.Body)));
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Fix(_config.Project);

            data
                .OfType<VstsService.Requests.Security.RemoveMembersData>()
                .ShouldNotContain(x => x.RemoveItemsJson.Contains(_rpa.TeamFoundationId));
        }


        [Fact]
        public void GivenProjectAdministratorsGroupProbablyDoesNotContainRabobankAdministratorsGroups_WhenFix_ThenThatGroupIsAdded()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client);
            
            var data = new List<object>();
            client
                .Post(Arg.Do<IVstsPostRequest<object>>(x => data.Add(x.Body)));
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Fix(_config.Project);

            data
                .OfType<VstsService.Requests.Security.AddMemberData>()
                .ShouldContain(x => 
                    x.ExistingUsersJson.Contains(_rpa.TeamFoundationId) && 
                    x.GroupsToJoinJson.Contains(_pa.TeamFoundationId));
        }
        
        [Fact]
        public void GivenRabobankProjectAdministratorsGroupDoesNotExist_WhenFix_ThenThatGroupIsCreated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client);
                            
            client
                .Post(Arg.Any<IVstsPostRequest<Response.ApplicationGroup>>())
                .Returns(_rpa);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Fix(_config.Project);
        }
        
        private static void InitializeApplicationGroupsLookup(IVstsRestClient client, params Response.ApplicationGroup[] groups)
        {
            client
                .Get(Arg.Is<IVstsRestRequest<Response.ApplicationGroups>>(x =>
                    x.Uri.Contains("ReadScopedApplicationGroupsJson")))
                .Returns(new Response.ApplicationGroups
                {
                    Identities = groups
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