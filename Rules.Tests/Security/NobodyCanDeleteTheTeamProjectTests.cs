using NSubstitute;
using SecurePipelineScan.Rules.Security;
using SecurePipelineScan.VstsService;
using SecurePipelineScan.VstsService.Permissions;
using SecurePipelineScan.VstsService.Requests;
using SecurePipelineScan.VstsService.Response;
using Shouldly;
using Xunit;
using ApplicationGroup = SecurePipelineScan.VstsService.Response.ApplicationGroup;
using Task = System.Threading.Tasks.Task;

namespace SecurePipelineScan.Rules.Tests.Security
{
    public class NobodyCanDeleteTheTeamProjectTests : IClassFixture<TestConfig>
    {
        private readonly TestConfig _config;

        private readonly ApplicationGroup _pa = new ApplicationGroup
            {FriendlyDisplayName = "Project Administrators", TeamFoundationId = "1234"};

        private readonly ApplicationGroup _rpa = new ApplicationGroup
            {FriendlyDisplayName = "Rabobank Project Administrators", TeamFoundationId = "adgasge"};

        private readonly Permission _deleteTeamProjectAllow = new Permission
        {
            DisplayName = "Delete team project", PermissionBit = 4, PermissionId = PermissionId.Allow,
            NamespaceId = SecurityNamespaceIds.Project,
            PermissionToken = "$PROJECT:vstfs:///Classification/TeamProject/53410703-e2e5-4238-9025-233bd7c811b3:"
        };

        public NobodyCanDeleteTheTeamProjectTests(TestConfig config)
        {
            _config = config;
        }

        [Fact]
        public async Task GivenProjectAdministratorsMembersEmpty_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.EvaluateAsync(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenProjectAdministratorsHasOnlyRabobankProjectAdministrators_WhenEvaluating_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client, _rpa);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.EvaluateAsync(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenProjectAdministratorsHasOtherMember_WhenEvaluate_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client, new ApplicationGroup());

            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.EvaluateAsync(_config.Project)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenOnlyProjectAdministratorHasPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.EvaluateAsync(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenContributorsHasPermissionToDeleteTeamProject_WhenEvaluate_ThenFalse()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, new ApplicationGroup {FriendlyDisplayName = "Contributors"});
            InitializeMembersLookup(client);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.EvaluateAsync(_config.Project)).ShouldBeFalse();
        }

        [Fact]
        public async Task GivenProjectAdministratorHasNoPermissionToDeleteTeamProject_WhenEvaluate_ThenTrue()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, new Permission());
            InitializeApplicationGroupsLookup(client, _pa);
            InitializeMembersLookup(client);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            (await rule.EvaluateAsync(_config.Project)).ShouldBeTrue();
        }

        [Fact]
        public async Task GivenProjectAdministratorsGroupContainsOtherMembers_WhenFix_ThenMembersAreRemoved()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client,
                new ApplicationGroup {TeamFoundationId = "asdf"},
                new ApplicationGroup {TeamFoundationId = "gsdgs"});

            var rule = new NobodyCanDeleteTheTeamProject(client);
            await rule.ReconcileAsync(_config.Project);

            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.EditMembersData, object>>(),
                    Arg.Is<VstsService.Requests.Security.RemoveMembersData>(x =>
                        x.RemoveItemsJson.Contains("asdf") &&
                        x.RemoveItemsJson.Contains("gsdgs") &&
                        x.GroupId == "1234"));
        }

        [Fact]
        public async Task
            GivenProjectAdministratorsGroupContainsOtherMembers_WhenFix_ThenMembersAreAddedToRabobankProjectAdministratorsGroup()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client,
                _rpa,
                new ApplicationGroup {TeamFoundationId = "asdf"},
                new ApplicationGroup {TeamFoundationId = "gsdgs"});

            var rule = new NobodyCanDeleteTheTeamProject(client);
            await rule.ReconcileAsync(_config.Project);

            await client
                .Received()
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.AddMemberData, object>>(),
                    Arg.Is<VstsService.Requests.Security.AddMemberData>(x =>
                        x.GroupsToJoinJson.Contains(_rpa.TeamFoundationId) &&
                        x.ExistingUsersJson.Contains("asdf") &&
                        x.ExistingUsersJson.Contains("gsdgs")));
        }

        [Fact]
        public async Task
            GivenProjectAdministratorsGroupContainsRabobankAdministratorsGroups_WhenFix_ThenThatMemberIsNotRemoved()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client, _rpa);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            await rule.ReconcileAsync(_config.Project);

            await client
                .DidNotReceive()
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.RemoveMembersData, object>>(),
                    Arg.Any<VstsService.Requests.Security.RemoveMembersData>());
        }


        [Fact]
        public async Task
            GivenProjectAdministratorsGroupProbablyDoesNotContainRabobankAdministratorsGroups_WhenFix_ThenThatGroupIsAdded()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa);
            InitializeMembersLookup(client);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            await rule.ReconcileAsync(_config.Project);

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
            await rule.ReconcileAsync(_config.Project);

            // Assert
            await client
                .DidNotReceive()
                .PostAsync(
                    Arg.Any<IVstsRequest<VstsService.Requests.Security.ManageGroupData, ApplicationGroup>>(),
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

            client
                .PostAsync(Arg.Any<IVstsRequest<VstsService.Requests.Security.ManageGroupData, ApplicationGroup>>(),
                    Arg.Any<VstsService.Requests.Security.ManageGroupData>())
                .Returns(_rpa);

            // Act
            var rule = new NobodyCanDeleteTheTeamProject(client);
            await rule.ReconcileAsync(_config.Project);

            // Assert
            await client
                .Received()
                .PostAsync(
                    Arg.Any<IVstsRequest<VstsService.Requests.Security.ManageGroupData, ApplicationGroup>>(),
                    Arg.Any<VstsService.Requests.Security.ManageGroupData>());
        }

        [Fact]
        public async Task
            GivenRabobankProjectAdministratorsHasInheritedAllowToDeleteTeamProject_WhenFix_ThenPermissionsAreUpdated()
        {
            var client = Substitute.For<IVstsRestClient>();
            InitializePermissions(client, _deleteTeamProjectAllow);
            InitializeApplicationGroupsLookup(client, _pa, _rpa,
                new ApplicationGroup {FriendlyDisplayName = "Contributors"});
            InitializeMembersLookup(client);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            await rule.ReconcileAsync(_config.Project);

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
            InitializeApplicationGroupsLookup(client, _pa, _rpa,
                new ApplicationGroup {FriendlyDisplayName = "Contributors", TeamFoundationId = "afewsf"});
            InitializeMembersLookup(client);

            var rule = new NobodyCanDeleteTheTeamProject(client);
            await rule.ReconcileAsync(_config.Project);

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

        private static void InitializeApplicationGroupsLookup(IVstsRestClient client, params ApplicationGroup[] groups)
        {
            client
                .GetAsync(Arg.Is<IVstsRequest<ApplicationGroups>>(x =>
                    x.Resource.Contains("ReadScopedApplicationGroupsJson")))
                .Returns(new ApplicationGroups
                {
                    Identities = groups
                });
        }

        private static void InitializeMembersLookup(IVstsRestClient client, params ApplicationGroup[] members)
        {
            client
                .GetAsync(Arg.Is<IVstsRequest<ApplicationGroups>>(x =>
                    x.Resource.Contains("ReadGroupMembers")))
                .Returns(new ApplicationGroups
                {
                    Identities = members
                });
        }

        private static void InitializePermissions(IVstsRestClient client, params Permission[] permissions)
        {
            client.GetAsync(Arg.Any<IVstsRequest<PermissionsProjectId>>())
                .Returns(new PermissionsProjectId
                    {Security = new PermissionsSetId {Permissions = permissions}});
        }
    }
}