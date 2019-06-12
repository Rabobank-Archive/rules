using System.Threading.Tasks;
using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Requests;
using Shouldly;
using Xunit;
using Response = SecurePipelineScan.VstsService.Response;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheTeamProjectTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;
        private readonly Response.ApplicationGroup _pa = new Response.ApplicationGroup {FriendlyDisplayName = "Project Administrators", TeamFoundationId = "1234"};
        private readonly Response.ApplicationGroup _rpa = new Response.ApplicationGroup{FriendlyDisplayName = "Rabobank Project Administrators", TeamFoundationId = "adgasge"};
        private readonly Response.Permission _deleteTeamProjectAllow = new Response.Permission{ DisplayName = "Delete team project", PermissionId = PermissionId.Allow, PermissionToken = "$PROJECT:vstfs:///Classification/TeamProject/53410703-e2e5-4238-9025-233bd7c811b3:"};

        public NobodyCanDeleteTheTeamProjectTests(TestConfig config)
        {
            _config = config;
        }
        
        [Fact]
        public async void EvaluateIntegrationTest()
        {
            var rule = new NobodyCanDeleteTheTeamProject(new VstsRestClient(_config.Organization, _config.Token));
            (await rule.Evaluate(_config.Project)).ShouldBeTrue();
        }
        
        [Fact]
        public async Task FixIntegrationTest()
        {
            var rule = new NobodyCanDeleteTheTeamProject(new VstsRestClient(_config.Organization, _config.Token));
            await rule.Reconcile(_config.Project);
        }
        
        [Fact]
        public async void GivenProjectAdministratorsMembersEmpty_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);           
            InitializeApplicationGroupsLookup(client, _pa);            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.Evaluate(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenProjectAdministratorsHasOnlyRabobankProjectAdministrators_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);           
            InitializeApplicationGroupsLookup(client, _pa);            
            InitializeMembersLookup(client, _rpa);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.Evaluate(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenProjectAdministratorsHasOtherMember_WhenEvaluate_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client, new Response.ApplicationGroup());
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.Evaluate(_config.Project)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenOnlyProjectAdministratorHasPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa);            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.Evaluate(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenContributorsHasPermissionToDeleteTeamProject_WhenEvaluate_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, new Response.ApplicationGroup {FriendlyDisplayName = "Contributors"});            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.Evaluate(_config.Project)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenProjectAdministratorHasNoPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, new Response.Permission());
            InitializeApplicationGroupsLookup(client, _pa);            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.Evaluate(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenProjectAdministratorsGroupContainsOtherMembers_WhenFix_ThenMembersAreRemoved()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client, 
                new Response.ApplicationGroup { TeamFoundationId = "asdf"},
                new Response.ApplicationGroup { TeamFoundationId = "gsdgs"});

            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);

            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.EditMembersData, object>>(),
                    Arg.Is<VstsService.Requests.Security.RemoveMembersData>(x => 
                        x.RemoveItemsJson.Contains("asdf") &&
                        x.RemoveItemsJson.Contains("gsdgs") &&
                        x.GroupId == "1234"));
        }
        
        [Fact]
        public async Task GivenProjectAdministratorsGroupContainsOtherMembers_WhenFix_ThenMembersAreAddedToRabobankProjectAdministratorsGroup()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client, 
                _rpa,
                new Response.ApplicationGroup { TeamFoundationId = "asdf"},
                new Response.ApplicationGroup { TeamFoundationId = "gsdgs"});
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);

            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.AddMemberData, object>>(),
                    Arg.Is<VstsService.Requests.Security.AddMemberData>(x => 
                        x.GroupsToJoinJson.Contains(_rpa.TeamFoundationId) &&
                        x.ExistingUsersJson.Contains("asdf") &&
                        x.ExistingUsersJson.Contains("gsdgs")));
        }
        
        [Fact]
        public async Task GivenProjectAdministratorsGroupContainsRabobankAdministratorsGroups_WhenFix_ThenThatMemberIsNotRemoved()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client, _rpa);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);

            await client
                .DidNotReceive()
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.RemoveMembersData, object>>(),
                    Arg.Any<VstsService.Requests.Security.RemoveMembersData>());
        }


        [Fact]
        public async Task GivenProjectAdministratorsGroupProbablyDoesNotContainRabobankAdministratorsGroups_WhenFix_ThenThatGroupIsAdded()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);

            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.AddMemberData, object>>(),
                    Arg.Is<VstsService.Requests.Security.AddMemberData>(x =>
                        x.ExistingUsersJson.Contains(_rpa.TeamFoundationId) &&
                        x.GroupsToJoinJson.Contains(_pa.TeamFoundationId)));
        }
        
        [Fact]
        public async Task GivenRabobankProjectAdministratorsGroupExists_WhenFix_ThenThatGroupIsNotCreated()
        {
            // Arrange
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client);
                      
            // Act
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);

            // Assert
            await client
                .DidNotReceive()
                .PostAsync(
                    Arg.Any<IVstsRequest<VstsService.Requests.Security.ManageGroupData, Response.ApplicationGroup>>(), 
                    Arg.Any<VstsService.Requests.Security.ManageGroupData>());
        }
        
        [Fact]
        public async Task GivenRabobankProjectAdministratorsGroupDoesNotExist_WhenFix_ThenThatGroupIsCreated()
        {
            // Arrange 
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client);
                            
            (await client
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.ManageGroupData, Response.ApplicationGroup>>(), Arg.Any<VstsService.Requests.Security.ManageGroupData>()))
                .Returns(_rpa);
            
            // Act
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);
            
            // Assert
            await client
                .Received()
                .PostAsync(
                    Arg.Any<IVstsRequest<VstsService.Requests.Security.ManageGroupData, Response.ApplicationGroup>>(), 
                    Arg.Any<VstsService.Requests.Security.ManageGroupData>());
        }
        
        [Fact]
        public async Task GivenRabobankProjectAdministratorsHasInheritedAllowToDeleteTeamProject_WhenFix_ThenPermissionsAreUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa, new Response.ApplicationGroup {FriendlyDisplayName = "Contributors"});            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);
            
            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Is<Permissions.UpdateWrapper>(x =>
                        x.UpdatePackage.Contains(_rpa.TeamFoundationId) &&
                        x.UpdatePackage.Contains(@"PermissionBit"":4") &&
                        x.UpdatePackage.Contains(@"PermissionId"":2")));
        }
        
        [Fact]
        public async Task GivenOtherMembersHavePermissionsToDeleteTeamProject_WhenFix_ThenPermissionsAreUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa, new Response.ApplicationGroup {FriendlyDisplayName = "Contributors", TeamFoundationId = "afewsf"});            
            InitializeMembersLookup(client);
            
            var rule = new NobodyCanDeleteTheTeamProject(client);
            rule.Reconcile(_config.Project);
            
            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Is<Permissions.UpdateWrapper>(x =>
                    x.UpdatePackage.Contains("afewsf") &&
                    x.UpdatePackage.Contains(@"PermissionId"":0")));
            
            await client
                .DidNotReceive()
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Is<Permissions.UpdateWrapper>(x =>
                        x.UpdatePackage.Contains(_pa.TeamFoundationId)));
            
            await client
                .Received(1)
                .PostAsync(Arg.Any<IVstsRequest<Permissions.UpdateWrapper, object>>(),
                    Arg.Is<Permissions.UpdateWrapper>(x =>
                        x.UpdatePackage.Contains(_rpa.TeamFoundationId))); // Only the DENY update

        }

        private static async Task InitializeApplicationGroupsLookup(IVstsRestClient client, params Response.ApplicationGroup[] groups)
        {
            (await client
                .GetAsync(Arg.Is<IVstsRequest<Response.ApplicationGroups>>(x =>
                    x.Resource.Contains("ReadScopedApplicationGroupsJson"))))
                .Returns(new Response.ApplicationGroups
                {
                    Identities = groups
                });
        }

        private static void InitializeMembersLookup(IVstsRestClient client, params Response.ApplicationGroup[] members)
        {
            client
                .GetAsync(Arg.Is<IVstsRequest<Response.ApplicationGroups>>(x =>
                    x.Resource.Contains("ReadGroupMembers")))
                .Returns(new Response.ApplicationGroups
                {
                    Identities = members
                });
        }
        
        private static void InitializePermissions(IVstsRestClient client, params Response.Permission[] permissions)
        {
            client.GetAsync(Arg.Any<IVstsRequest<Response.PermissionsProjectId>>())
                .Returns(new Response.PermissionsProjectId
                    {Security = new Response.PermissionsSetId {Permissions = permissions}});
        }
    }
}